' Data Recovery Tool Usage Examples
' 
' This file demonstrates how to use the DataRecoveryCore.dll library
' in your VB.NET applications for intermediate-level data recovery.

Imports DataRecoveryCore.Recovery
Imports DataRecoveryCore.FileSignatures
Imports Microsoft.Extensions.Logging

Module UsageExamples

    ' Example 1: Basic File Recovery
    Public Async Function BasicRecoveryExample() As Task
        ' Setup logging
        Using loggerFactory = LoggerFactory.Create(Sub(builder) builder.AddConsole())
            Dim logger = loggerFactory.CreateLogger(Of RecoveryEngine)
            
            ' Initialize recovery engine
            Using recovery As New RecoveryEngine(logger)
                ' Open physical drive 0 (requires admin privileges)
                If recovery.Initialize(driveNumber:=0) Then
                    Console.WriteLine("Recovery engine initialized successfully")
                    
                    ' Recover deleted image files
                    Dim result = Await recovery.RecoverFilesAsync(
                        mode:=RecoveryEngine.RecoveryMode.Combined,
                        targetExtensions:={"jpg", "png", "gif"},
                        maxScanSize:=1024 * 1024 * 1024,  ' 1GB scan limit
                        outputDirectory:="C:\RecoveredFiles"
                    )
                    
                    Console.WriteLine($"Recovery completed:")
                    Console.WriteLine($"- Files found: {result.TotalFilesFound}")
                    Console.WriteLine($"- Data recovered: {result.TotalBytesRecovered / 1024 / 1024} MB")
                    Console.WriteLine($"- Scan time: {result.ScanDuration.TotalMinutes:F1} minutes")
                Else
                    Console.WriteLine("Failed to initialize recovery engine")
                End If
            End Using
        End Using
    End Function

    ' Example 2: Document Recovery with High Confidence
    Public Async Function DocumentRecoveryExample() As Task
        Using loggerFactory = LoggerFactory.Create(Sub(builder) builder.AddConsole())
            Dim logger = loggerFactory.CreateLogger(Of RecoveryEngine)
            
            Using recovery As New RecoveryEngine(logger)
                If recovery.Initialize(driveNumber:=1) Then ' Second drive
                    
                    ' Focus on Office documents only
                    Dim result = Await recovery.RecoverFilesAsync(
                        mode:=RecoveryEngine.RecoveryMode.FileSystemOnly,
                        targetExtensions:={"docx", "xlsx", "pptx", "pdf"},
                        outputDirectory:="C:\RecoveredDocuments"
                    )
                    
                    ' Filter results by confidence level
                    Dim highConfidenceFiles = result.RecoveredFiles.Where(
                        Function(f) f.IsSuccessful AndAlso f.FileInfo.ConfidenceLevel > 0.8
                    ).ToList()
                    
                    Console.WriteLine($"High confidence recoveries: {highConfidenceFiles.Count}")
                    
                    For Each file In highConfidenceFiles
                        Console.WriteLine($"- {file.FileInfo.FileName} ({file.FileInfo.FileSize:N0} bytes)")
                    Next
                End If
            End Using
        End Using
    End Function

    ' Example 3: Deep Scan for Specific File Types
    Public Async Function DeepScanExample() As Task
        Using loggerFactory = LoggerFactory.Create(Sub(builder) 
            builder.AddConsole().SetMinimumLevel(LogLevel.Information)
        End Sub)
            Dim logger = loggerFactory.CreateLogger(Of RecoveryEngine)
            
            Using recovery As New RecoveryEngine(logger)
                If recovery.Initialize(driveNumber:=0) Then
                    
                    ' Perform deep scan for video files
                    Dim result = Await recovery.RecoverFilesAsync(
                        mode:=RecoveryEngine.RecoveryMode.DeepScan,
                        targetExtensions:={"mp4", "avi", "mov", "wmv"},
                        maxScanSize:=50L * 1024 * 1024 * 1024, ' 50GB limit
                        outputDirectory:="C:\RecoveredVideos"
                    )
                    
                    ' Generate detailed report
                    Console.WriteLine($"Deep Scan Results:")
                    Console.WriteLine($"- Scan duration: {result.ScanDuration}")
                    Console.WriteLine($"- Files found: {result.TotalFilesFound}")
                    Console.WriteLine($"- Success rate: {(result.RecoveredFiles.Count(Function(f) f.IsSuccessful) * 100.0 / result.TotalFilesFound):F1}%")
                    
                    ' Group by file type
                    Dim fileGroups = result.RecoveredFiles.
                        Where(Function(f) f.IsSuccessful).
                        GroupBy(Function(f) f.FileInfo.FileExtension).
                        OrderByDescending(Function(g) g.Count())
                    
                    Console.WriteLine($"File type breakdown:")
                    For Each group In fileGroups
                        Console.WriteLine($"  {group.Key}: {group.Count()} files")
                    Next
                End If
            End Using
        End Using
    End Function

    ' Example 4: File Signature Analysis Only
    Public Sub FileSignatureExample()
        Using loggerFactory = LoggerFactory.Create(Sub(builder) builder.AddConsole())
            Dim logger = loggerFactory.CreateLogger(Of FileSignatureAnalyzer)
            Dim analyzer As New FileSignatureAnalyzer(logger)
            
            ' Simulate raw data with embedded JPEG
            Dim testData(2048) As Byte
            ' Add JPEG signature at offset 1000
            testData(1000) = &HFF
            testData(1001) = &HD8
            testData(1002) = &HFF
            testData(1003) = &HE0
            
            ' Analyze data block
            Dim detectedFiles = analyzer.AnalyzeDataBlock(testData, baseOffset:=0)
            
            Console.WriteLine($"File signatures detected: {detectedFiles.Count}")
            For Each detected In detectedFiles
                Console.WriteLine($"- {detected.Signature.Extension} at offset {detected.StartOffset:X}")
                Console.WriteLine($"  Confidence: {detected.ConfidenceLevel:P1}")
                Console.WriteLine($"  Category: {detected.Signature.Category}")
            Next
            
            ' Get all supported signatures by category
            Dim imageSignatures = analyzer.GetSignaturesByCategory(FileCategory.Image)
            Console.WriteLine($"Supported image formats: {imageSignatures.Count}")
            For Each sig In imageSignatures
                Console.WriteLine($"  .{sig.Extension} - {sig.Description}")
            Next
        End Using
    End Sub

    ' Example 5: Check System Requirements
    Public Sub SystemRequirementsCheck()
        Console.WriteLine("=== System Requirements Check ===")
        
        ' Check admin privileges
        If DataRecoveryCore.DiskAccess.DiskAccessManager.HasAdministratorPrivileges() Then
            Console.WriteLine("✓ Administrator privileges: Available")
        Else
            Console.WriteLine("✗ Administrator privileges: REQUIRED")
            Console.WriteLine("  Please run application as Administrator")
        End If
        
        ' Check .NET version
        Console.WriteLine($"✓ .NET Version: {Environment.Version}")
        
        ' Check platform
        Console.WriteLine($"✓ Platform: {Environment.OSVersion}")
        
        If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
            Console.WriteLine("✗ Windows platform required for disk access")
        End If
        
        ' Check available memory
        Dim totalMemory = GC.GetTotalMemory(forceFullCollection:=False)
        Console.WriteLine($"✓ Available memory: {totalMemory / 1024 / 1024:N0} MB")
    End Sub

    ' Main entry point for examples
    Sub Main()
        Console.WriteLine("Data Recovery Tool - Usage Examples")
        Console.WriteLine("=" * 40)
        
        Try
            ' Run system check first
            SystemRequirementsCheck()
            Console.WriteLine()
            
            ' Note: Actual recovery examples require admin privileges and real drives
            Console.WriteLine("Note: Recovery examples require:")
            Console.WriteLine("1. Administrator privileges")
            Console.WriteLine("2. Valid physical drives")
            Console.WriteLine("3. Proper output directories")
            Console.WriteLine()
            
            ' Run file signature example (safe)
            Console.WriteLine("Running file signature detection example...")
            FileSignatureExample()
            
        Catch ex As Exception
            Console.WriteLine($"Error: {ex.Message}")
        End Try
    End Sub

End Module
