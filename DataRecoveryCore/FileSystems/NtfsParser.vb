Imports System.Runtime.InteropServices
Imports System.Text
Imports Microsoft.Extensions.Logging
Imports DataRecoveryCore.DiskAccess

Namespace FileSystems

    ''' <summary>
    ''' NTFS file system parser for deleted file recovery
    ''' Handles Master File Table (MFT) analysis and file record parsing
    ''' </summary>
    Public Class NtfsParser

        Private ReadOnly _logger As ILogger(Of NtfsParser)
        Private ReadOnly _diskAccess As DiskAccessManager
        Private _bootSector As NtfsBootSector
        Private _mftStartCluster As Long
        Private _bytesPerSector As Integer
        Private _sectorsPerCluster As Integer
        Private _bytesPerCluster As Integer

        <StructLayout(LayoutKind.Sequential, Pack:=1)>
        Public Structure NtfsBootSector
            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)>
            Public JumpInstruction As Byte()
            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)>
            Public OemId As Byte()
            Public BytesPerSector As UShort
            Public SectorsPerCluster As Byte
            Public ReservedSectors As UShort
            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)>
            Public Unused1 As Byte()
            Public Unused2 As UShort
            Public MediaDescriptor As Byte
            Public Unused3 As UShort
            Public SectorsPerTrack As UShort
            Public NumberOfHeads As UShort
            Public HiddenSectors As UInteger
            Public Unused4 As UInteger
            Public Unused5 As UInteger
            Public TotalSectors As ULong
            Public MftStartCluster As ULong
            Public MftMirrorStartCluster As ULong
            Public ClustersPerMftRecord As Integer
            Public Unused6 As UInteger
            Public ClustersPerIndexBuffer As Integer
            Public Unused7 As UInteger
            Public VolumeSerialNumber As ULong
            Public Checksum As UInteger
            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=426)>
            Public BootCode As Byte()
            Public Signature As UShort ' Should be 0xAA55
        End Structure

        <StructLayout(LayoutKind.Sequential, Pack:=1)>
        Public Structure MftRecordHeader
            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)>
            Public Signature As Byte() ' "FILE"
            Public UpdateSequenceOffset As UShort
            Public UpdateSequenceSize As UShort
            Public LogFileSequenceNumber As ULong
            Public SequenceNumber As UShort
            Public HardLinkCount As UShort
            Public FirstAttributeOffset As UShort
            Public Flags As UShort ' 0x01 = In use, 0x02 = Directory
            Public UsedSize As UInteger
            Public AllocatedSize As UInteger
            Public FileReference As ULong
            Public NextAttributeId As UShort
            Public Unused As UShort
            Public RecordNumber As UInteger
        End Structure

        <StructLayout(LayoutKind.Sequential, Pack:=1)>
        Public Structure AttributeHeader
            Public AttributeType As UInteger
            Public Length As UInteger
            Public NonResident As Byte
            Public NameLength As Byte
            Public NameOffset As UShort
            Public Flags As UShort
            Public AttributeId As UShort
        End Structure

        Public Enum NtfsAttributeType As UInteger
            StandardInformation = &H10
            AttributeList = &H20
            FileName = &H30
            ObjectId = &H40
            SecurityDescriptor = &H50
            VolumeName = &H60
            VolumeInformation = &H70
            Data = &H80
            IndexRoot = &H90
            IndexAllocation = &HA0
            Bitmap = &HB0
            ReparsePoint = &HC0
            EAInformation = &HD0
            EA = &HE0
            PropertySet = &HF0
            LoggedUtilityStream = &H100
        End Enum

        Public Class NtfsFileRecord
            Public Property RecordNumber As Long
            Public Property IsDeleted As Boolean
            Public Property IsDirectory As Boolean
            Public Property FileName As String
            Public Property FileSize As Long
            Public Property CreatedTime As DateTime
            Public Property ModifiedTime As DateTime
            Public Property AccessedTime As DateTime
            Public Property DataRuns As List(Of DataRun)
            Public Property Confidence As Double

            Public Sub New()
                DataRuns = New List(Of DataRun)
            End Sub
        End Class

        Public Class DataRun
            Public Property StartCluster As Long
            Public Property ClusterCount As Long
            Public Property IsCompressed As Boolean

            Public Sub New(startCluster As Long, clusterCount As Long)
                Me.StartCluster = startCluster
                Me.ClusterCount = clusterCount
            End Sub
        End Class

        Public Sub New(logger As ILogger(Of NtfsParser), diskAccess As DiskAccessManager)
            _logger = logger ?? throw New ArgumentNullException(NameOf(logger))
            _diskAccess = diskAccess ?? throw New ArgumentNullException(NameOf(diskAccess))
        End Sub

        ''' <summary>
        ''' Initializes NTFS parser by reading boot sector
        ''' </summary>
        Public Function Initialize() As Boolean
            Try
                ' Read boot sector (sector 0)
                Dim bootSectorData As Byte() = _diskAccess.ReadSector(0, 512)
                If bootSectorData Is Nothing Then
                    _logger.LogError("Failed to read boot sector")
                    Return False
                End If

                ' Parse boot sector
                Dim handle As GCHandle = GCHandle.Alloc(bootSectorData, GCHandleType.Pinned)
                Try
                    _bootSector = Marshal.PtrToStructure(Of NtfsBootSector)(handle.AddrOfPinnedObject())
                Finally
                    handle.Free()
                End Try

                ' Validate NTFS signature
                Dim oemString As String = Encoding.ASCII.GetString(_bootSector.OemId).Trim()
                If Not oemString.StartsWith("NTFS") Then
                    _logger.LogError($"Invalid NTFS signature: {oemString}")
                    Return False
                End If

                ' Extract file system parameters
                _bytesPerSector = _bootSector.BytesPerSector
                _sectorsPerCluster = _bootSector.SectorsPerCluster
                _bytesPerCluster = _bytesPerSector * _sectorsPerCluster
                _mftStartCluster = CLng(_bootSector.MftStartCluster)

                _logger.LogInformation($"NTFS initialized - Bytes/Sector: {_bytesPerSector}, Sectors/Cluster: {_sectorsPerCluster}, MFT Start: {_mftStartCluster}")
                Return True

            Catch ex As Exception
                _logger.LogError(ex, "Failed to initialize NTFS parser")
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Scans MFT for deleted file records
        ''' </summary>
        Public Function ScanDeletedFiles() As List(Of NtfsFileRecord)
            Dim deletedFiles As New List(Of NtfsFileRecord)

            Try
                _logger.LogInformation("Starting MFT scan for deleted files...")

                ' Calculate MFT location
                Dim mftSectorStart As Long = _mftStartCluster * _sectorsPerCluster
                Dim recordSize As Integer = If(_bootSector.ClustersPerMftRecord > 0, _bootSector.ClustersPerMftRecord * _bytesPerCluster, 1024)

                ' Scan MFT records (start from record 16, skip system records)
                Dim currentRecord As Long = 16
                Dim consecutiveErrors As Integer = 0
                Const MaxConsecutiveErrors As Integer = 100

                While consecutiveErrors < MaxConsecutiveErrors
                    Try
                        Dim recordSector As Long = mftSectorStart + (currentRecord * recordSize) \ _bytesPerSector
                        Dim recordData As Byte() = _diskAccess.ReadSectors(recordSector, recordSize \ _bytesPerSector, _bytesPerSector)

                        If recordData IsNot Nothing Then
                            Dim fileRecord As NtfsFileRecord = ParseMftRecord(recordData, currentRecord)
                            If fileRecord IsNot Nothing AndAlso fileRecord.IsDeleted Then
                                deletedFiles.Add(fileRecord)
                                _logger.LogDebug($"Found deleted file: {fileRecord.FileName} (Record #{fileRecord.RecordNumber})")
                            End If
                            consecutiveErrors = 0
                        Else
                            consecutiveErrors += 1
                        End If

                        currentRecord += 1

                        ' Log progress every 10000 records
                        If currentRecord Mod 10000 = 0 Then
                            _logger.LogInformation($"Scanned {currentRecord} MFT records, found {deletedFiles.Count} deleted files")
                        End If

                    Catch ex As Exception
                        _logger.LogWarning(ex, $"Error processing MFT record {currentRecord}")
                        consecutiveErrors += 1
                        currentRecord += 1
                    End Try
                End While

                _logger.LogInformation($"MFT scan complete. Found {deletedFiles.Count} deleted files from {currentRecord} records")

            Catch ex As Exception
                _logger.LogError(ex, "Error during MFT scan")
            End Try

            Return deletedFiles
        End Function

        ''' <summary>
        ''' Parses individual MFT record
        ''' </summary>
        Private Function ParseMftRecord(data As Byte(), recordNumber As Long) As NtfsFileRecord
            Try
                ' Validate record signature
                If data.Length < Marshal.SizeOf(Of MftRecordHeader) OrElse
                   data(0) <> &H46 OrElse data(1) <> &H49 OrElse data(2) <> &H4C OrElse data(3) <> &H45 Then ' "FILE"
                    Return Nothing
                End If

                ' Parse record header
                Dim handle As GCHandle = GCHandle.Alloc(data, GCHandleType.Pinned)
                Dim header As MftRecordHeader
                Try
                    header = Marshal.PtrToStructure(Of MftRecordHeader)(handle.AddrOfPinnedObject())
                Finally
                    handle.Free()
                End Try

                ' Create file record
                Dim fileRecord As New NtfsFileRecord With {
                    .RecordNumber = recordNumber,
                    .IsDeleted = (header.Flags And &H1) = 0, ' Not in use = deleted
                    .IsDirectory = (header.Flags And &H2) <> 0
                }

                ' Skip if record is still active
                If Not fileRecord.IsDeleted Then
                    Return Nothing
                End If

                ' Parse attributes
                Dim offset As Integer = header.FirstAttributeOffset
                While offset < data.Length - 8 AndAlso offset < header.UsedSize
                    Dim attrHeader As AttributeHeader
                    If offset + Marshal.SizeOf(Of AttributeHeader) <= data.Length Then
                        handle = GCHandle.Alloc(data, GCHandleType.Pinned)
                        Try
                            attrHeader = Marshal.PtrToStructure(Of AttributeHeader)(handle.AddrOfPinnedObject() + offset)
                        Finally
                            handle.Free()
                        End Try

                        ' End of attributes marker
                        If attrHeader.AttributeType = &HFFFFFFFF Then
                            Exit While
                        End If

                        ' Process attribute
                        Select Case CType(attrHeader.AttributeType, NtfsAttributeType)
                            Case NtfsAttributeType.FileName
                                ParseFileNameAttribute(data, offset, fileRecord)
                            Case NtfsAttributeType.StandardInformation
                                ParseStandardInformationAttribute(data, offset, fileRecord)
                            Case NtfsAttributeType.Data
                                ParseDataAttribute(data, offset, fileRecord)
                        End Select

                        offset += CInt(attrHeader.Length)
                    Else
                        Exit While
                    End If
                End While

                ' Calculate confidence based on data availability
                fileRecord.Confidence = CalculateRecordConfidence(fileRecord)

                Return fileRecord

            Catch ex As Exception
                _logger.LogWarning(ex, $"Error parsing MFT record {recordNumber}")
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Parses filename attribute
        ''' </summary>
        Private Sub ParseFileNameAttribute(data As Byte(), offset As Integer, fileRecord As NtfsFileRecord)
            Try
                If offset + 24 >= data.Length Then Return

                ' Skip resident attribute header (24 bytes)
                Dim nameInfoOffset As Integer = offset + 24

                If nameInfoOffset + 66 >= data.Length Then Return

                ' Parse filename information
                Dim parentRef As ULong = BitConverter.ToUInt64(data, nameInfoOffset)
                Dim createdTime As ULong = BitConverter.ToUInt64(data, nameInfoOffset + 8)
                Dim modifiedTime As ULong = BitConverter.ToUInt64(data, nameInfoOffset + 16)
                Dim accessedTime As ULong = BitConverter.ToUInt64(data, nameInfoOffset + 24)
                Dim allocatedSize As ULong = BitConverter.ToUInt64(data, nameInfoOffset + 32)
                Dim realSize As ULong = BitConverter.ToUInt64(data, nameInfoOffset + 40)
                Dim flags As UInteger = BitConverter.ToUInt32(data, nameInfoOffset + 48)
                Dim nameLength As Byte = data(nameInfoOffset + 64)
                Dim nameNamespace As Byte = data(nameInfoOffset + 65)

                ' Extract filename (UTF-16)
                If nameInfoOffset + 66 + (nameLength * 2) <= data.Length Then
                    Dim nameBytes(nameLength * 2 - 1) As Byte
                    Array.Copy(data, nameInfoOffset + 66, nameBytes, 0, nameLength * 2)
                    fileRecord.FileName = Encoding.Unicode.GetString(nameBytes)
                End If

                ' Convert FILETIME to DateTime
                fileRecord.CreatedTime = FileTimeToDateTime(createdTime)
                fileRecord.ModifiedTime = FileTimeToDateTime(modifiedTime)
                fileRecord.AccessedTime = FileTimeToDateTime(accessedTime)
                fileRecord.FileSize = CLng(realSize)

            Catch ex As Exception
                _logger.LogDebug(ex, "Error parsing filename attribute")
            End Try
        End Sub

        ''' <summary>
        ''' Parses standard information attribute
        ''' </summary>
        Private Sub ParseStandardInformationAttribute(data As Byte(), offset As Integer, fileRecord As NtfsFileRecord)
            Try
                If offset + 72 >= data.Length Then Return

                ' Skip resident attribute header (24 bytes)
                Dim infoOffset As Integer = offset + 24

                ' Parse standard information
                Dim createdTime As ULong = BitConverter.ToUInt64(data, infoOffset)
                Dim modifiedTime As ULong = BitConverter.ToUInt64(data, infoOffset + 8)
                Dim mftModifiedTime As ULong = BitConverter.ToUInt64(data, infoOffset + 16)
                Dim accessedTime As ULong = BitConverter.ToUInt64(data, infoOffset + 24)
                Dim attributes As UInteger = BitConverter.ToUInt32(data, infoOffset + 32)

                ' Update timestamps if not already set
                If fileRecord.CreatedTime = DateTime.MinValue Then
                    fileRecord.CreatedTime = FileTimeToDateTime(createdTime)
                End If
                If fileRecord.ModifiedTime = DateTime.MinValue Then
                    fileRecord.ModifiedTime = FileTimeToDateTime(modifiedTime)
                End If
                If fileRecord.AccessedTime = DateTime.MinValue Then
                    fileRecord.AccessedTime = FileTimeToDateTime(accessedTime)
                End If

            Catch ex As Exception
                _logger.LogDebug(ex, "Error parsing standard information attribute")
            End Try
        End Sub

        ''' <summary>
        ''' Parses data attribute to extract file location
        ''' </summary>
        Private Sub ParseDataAttribute(data As Byte(), offset As Integer, fileRecord As NtfsFileRecord)
            Try
                If offset + 16 >= data.Length Then Return

                Dim attrHeader As AttributeHeader
                Dim handle As GCHandle = GCHandle.Alloc(data, GCHandleType.Pinned)
                Try
                    attrHeader = Marshal.PtrToStructure(Of AttributeHeader)(handle.AddrOfPinnedObject() + offset)
                Finally
                    handle.Free()
                End Try

                ' Handle non-resident data (actual file content stored in clusters)
                If attrHeader.NonResident <> 0 Then
                    ParseNonResidentDataRuns(data, offset, fileRecord)
                End If

            Catch ex As Exception
                _logger.LogDebug(ex, "Error parsing data attribute")
            End Try
        End Sub

        ''' <summary>
        ''' Parses data runs for non-resident files
        ''' </summary>
        Private Sub ParseNonResidentDataRuns(data As Byte(), offset As Integer, fileRecord As NtfsFileRecord)
            Try
                ' Skip to data runs (non-resident header is 64 bytes)
                Dim runOffset As Integer = offset + 64
                If runOffset >= data.Length Then Return

                Dim currentCluster As Long = 0

                While runOffset < data.Length
                    Dim runHeader As Byte = data(runOffset)
                    If runHeader = 0 Then Exit While ' End of runs

                    Dim lengthBytes As Integer = runHeader And &HF
                    Dim offsetBytes As Integer = (runHeader >> 4) And &HF

                    runOffset += 1

                    If runOffset + lengthBytes + offsetBytes > data.Length Then Exit While

                    ' Extract run length
                    Dim runLength As Long = 0
                    For i As Integer = 0 To lengthBytes - 1
                        runLength = runLength Or (CLng(data(runOffset + i)) << (i * 8))
                    Next
                    runOffset += lengthBytes

                    ' Extract cluster offset
                    Dim clusterOffset As Long = 0
                    If offsetBytes > 0 Then
                        For i As Integer = 0 To offsetBytes - 1
                            clusterOffset = clusterOffset Or (CLng(data(runOffset + i)) << (i * 8))
                        Next
                        
                        ' Handle signed offset
                        If (data(runOffset + offsetBytes - 1) And &H80) <> 0 Then
                            ' Sign extend for negative offset
                            For i As Integer = offsetBytes To 7
                                clusterOffset = clusterOffset Or (&HFFL << (i * 8))
                            Next
                        End If
                        
                        currentCluster += clusterOffset
                    End If
                    runOffset += offsetBytes

                    ' Add data run
                    If runLength > 0 AndAlso currentCluster > 0 Then
                        fileRecord.DataRuns.Add(New DataRun(currentCluster, runLength))
                    End If
                End While

            Catch ex As Exception
                _logger.LogDebug(ex, "Error parsing data runs")
            End Try
        End Sub

        ''' <summary>
        ''' Converts Windows FILETIME to DateTime
        ''' </summary>
        Private Function FileTimeToDateTime(fileTime As ULong) As DateTime
            Try
                If fileTime = 0 Then Return DateTime.MinValue
                Return DateTime.FromFileTimeUtc(CLng(fileTime))
            Catch
                Return DateTime.MinValue
            End Try
        End Function

        ''' <summary>
        ''' Calculates confidence score for recovered file record
        ''' </summary>
        Private Function CalculateRecordConfidence(fileRecord As NtfsFileRecord) As Double
            Dim confidence As Double = 0.3 ' Base confidence for deleted file

            ' Increase confidence based on available information
            If Not String.IsNullOrEmpty(fileRecord.FileName) Then confidence += 0.3
            If fileRecord.FileSize > 0 Then confidence += 0.2
            If fileRecord.DataRuns.Count > 0 Then confidence += 0.2
            If fileRecord.CreatedTime <> DateTime.MinValue Then confidence += 0.1
            If fileRecord.ModifiedTime <> DateTime.MinValue Then confidence += 0.1

            Return Math.Min(1.0, confidence)
        End Function

        ''' <summary>
        ''' Recovers file data using data runs
        ''' </summary>
        Public Function RecoverFileData(fileRecord As NtfsFileRecord) As Byte()
            Try
                If fileRecord.DataRuns.Count = 0 OrElse fileRecord.FileSize <= 0 Then
                    Return Nothing
                End If

                Dim recoveredData As New List(Of Byte)
                Dim totalBytesToRead As Long = fileRecord.FileSize

                For Each run As DataRun In fileRecord.DataRuns
                    If totalBytesToRead <= 0 Then Exit For

                    Dim startSector As Long = run.StartCluster * _sectorsPerCluster
                    Dim sectorsToRead As Integer = CInt(Math.Min(run.ClusterCount * _sectorsPerCluster, 
                                                                (totalBytesToRead + _bytesPerSector - 1) \ _bytesPerSector))

                    Dim runData As Byte() = _diskAccess.ReadSectors(startSector, sectorsToRead, _bytesPerSector)
                    If runData IsNot Nothing Then
                        Dim bytesToCopy As Integer = CInt(Math.Min(runData.Length, totalBytesToRead))
                        Dim copyData(bytesToCopy - 1) As Byte
                        Array.Copy(runData, 0, copyData, 0, bytesToCopy)
                        recoveredData.AddRange(copyData)
                        totalBytesToRead -= bytesToCopy
                    End If
                Next

                Return recoveredData.ToArray()

            Catch ex As Exception
                _logger.LogError(ex, $"Error recovering file data for {fileRecord.FileName}")
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Gets file system parameters
        ''' </summary>
        Public ReadOnly Property BytesPerSector As Integer
            Get
                Return _bytesPerSector
            End Get
        End Property

        Public ReadOnly Property SectorsPerCluster As Integer
            Get
                Return _sectorsPerCluster
            End Get
        End Property

        Public ReadOnly Property BytesPerCluster As Integer
            Get
                Return _bytesPerCluster
            End Get
        End Property

    End Class

End Namespace
