Imports System.IO
Imports DataRecoveryCore.Recovery

Namespace UserInterface

    ''' <summary>
    ''' User-friendly console interface with visual guides and step-by-step recovery process
    ''' </summary>
    Public Class ConsoleUI

        Private Const BorderChar As String = "═"
        Private Const VerticalChar As String = "║"
        Private Const TopLeftCorner As String = "╔"
        Private Const TopRightCorner As String = "╗"
        Private Const BottomLeftCorner As String = "╚"
        Private Const BottomRightCorner As String = "╝"
        Private Const HorizontalSeparator As String = "╠═══════════════════════════════════════════════════════════════════════════╣"

        ' Unicode/Emoji support detection
        Private Shared _supportsUnicode As Boolean? = Nothing

        Private Shared Function SupportsUnicode() As Boolean
            If _supportsUnicode Is Nothing Then
                Try
                    ' Test if Unicode output works by checking encoding
                    _supportsUnicode = (System.Console.OutputEncoding.CodePage = 65001) OrElse ' UTF-8
                                     (System.Console.OutputEncoding.CodePage = 1200)   ' UTF-16
                Catch
                    _supportsUnicode = False
                End Try
            End If
            Return _supportsUnicode.Value
        End Function

        Private Shared Function GetIcon(unicodeIcon As String, fallback As String) As String
            Return If(SupportsUnicode(), unicodeIcon, fallback)
        End Function

        Public Shared Sub ShowWelcomeScreen()
            System.Console.Clear()
            SetConsoleColors()
            
            ' Expand console window for stunning display
            Try
                If System.Console.LargestWindowWidth >= 100 Then
                    System.Console.SetWindowSize(100, 35)
                End If
            Catch
                ' Ignore window sizing errors
            End Try
            
            System.Console.WriteLine()
            
            ' Stunning ASCII Art Logo inspired by Claude
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.WriteLine("    ███████╗██╗██╗     ███████╗    ██████╗ ███████╗ ██████╗ █████╗ ██╗     ██╗     ")
            System.Console.WriteLine("    ██╔════╝██║██║     ██╔════╝    ██╔══██╗██╔════╝██╔════╝██╔══██╗██║     ██║     ")
            System.Console.WriteLine("    █████╗  ██║██║     █████╗      ██████╔╝█████╗  ██║     ███████║██║     ██║     ")
            System.Console.WriteLine("    ██╔══╝  ██║██║     ██╔══╝      ██╔══██╗██╔══╝  ██║     ██╔══██║██║     ██║     ")
            System.Console.WriteLine("    ██║     ██║███████╗███████╗    ██║  ██║███████╗╚██████╗██║  ██║███████╗███████╗")
            System.Console.WriteLine("    ╚═╝     ╚═╝╚══════╝╚══════╝    ╚═╝  ╚═╝╚══════╝ ╚═════╝╚═╝  ╚═╝╚══════╝╚══════╝")
            System.Console.WriteLine()
            
            ' Professional subtitle with elegant typography
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("                          Professional Data Recovery Solution v1.0")
            System.Console.WriteLine("                         ═══════════════════════════════════════════")
            System.Console.WriteLine()
            
            ' Elegant feature showcase with modern styling
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.WriteLine("    ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓")
            System.Console.WriteLine("    ┃                                CORE CAPABILITIES                                ┃")
            System.Console.WriteLine("    ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine($"      {GetIcon("�", "[>]")}  Advanced Recovery Engine       ║  Raw disk sector-level analysis")
            System.Console.WriteLine($"      {GetIcon("💾", "[D]")}  Multi-Format Support           ║  30+ file signatures detected")
            System.Console.WriteLine($"      {GetIcon("🛡️", "[S]")}  NTFS Deep Analysis             ║  Master File Table parsing")
            System.Console.WriteLine($"      {GetIcon("�", "[?]")}  Intelligent Scanning           ║  Multiple recovery strategies")
            System.Console.WriteLine($"      {GetIcon("⚡", "[F]")}  High-Performance I/O           ║  Async operations & memory efficient")
            System.Console.WriteLine()
            
            ' Security notice with elegant styling
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine("    ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓")
            System.Console.WriteLine("    ┃                              SECURITY REQUIREMENTS                             ┃")
            System.Console.WriteLine("    ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛")
            System.Console.WriteLine()
            System.Console.ForegroundColor = ConsoleColor.Red
            System.Console.WriteLine($"      {GetIcon("⚠️", "[!]")}  Administrator privileges required for raw disk access")
            System.Console.WriteLine($"      {GetIcon("⚠️", "[!]")}  Only use on systems you own or have explicit authorization")
            System.Console.WriteLine($"      {GetIcon("⚠️", "[!]")}  Data recovery operations may take considerable time")
            System.Console.WriteLine()
            
            ' Call to action with beautiful styling
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.WriteLine("    ╭─────────────────────────────────────────────────────────────────────────────╮")
            System.Console.WriteLine("    │                    Press any key to launch the Recovery Wizard                 │")
            System.Console.WriteLine("    ╰─────────────────────────────────────────────────────────────────────────────╯")
            
            System.Console.ResetColor()
            System.Console.ReadKey(True)
        End Sub

        Public Shared Function ShowMainMenu() As Integer
            System.Console.Clear()
            SetConsoleColors()
            System.Console.WriteLine()
            
            ' Beautiful header with gradient-style border
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.WriteLine("    ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓")
            System.Console.WriteLine("    ┃                              RECOVERY MODE SELECTION                           ┃")
            System.Console.WriteLine("    ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.WriteLine("    Choose your recovery approach based on your specific data loss scenario:")
            System.Console.WriteLine()
            
            ' Option 1 - Quick Recovery
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.WriteLine($"    ┌── [1] {GetIcon("🚀", "[Q]")} QUICK RECOVERY ────────────────────────────────────────────────┐")
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("    │    Fast scan using file system metadata (NTFS/FAT)                    │")
            System.Console.WriteLine("    │    ✓ Best for recently deleted files                                  │")
            System.Console.WriteLine("    │    ⏱ Estimated time: 2-10 minutes                                    │")
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.WriteLine("    └────────────────────────────────────────────────────────────────────────┘")
            System.Console.WriteLine()
            
            ' Option 2 - Deep Signature Scan
            System.Console.ForegroundColor = ConsoleColor.Blue
            System.Console.WriteLine($"    ┌── [2] {GetIcon("🔍", "[D]")} DEEP SIGNATURE SCAN ───────────────────────────────────────────┐")
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("    │    Sector-by-sector analysis with file signature detection            │")
            System.Console.WriteLine("    │    ✓ Recovers files even after format/corruption                     │")
            System.Console.WriteLine("    │    ⏱ Estimated time: 30 minutes - 2 hours                           │")
            System.Console.ForegroundColor = ConsoleColor.Blue
            System.Console.WriteLine("    └────────────────────────────────────────────────────────────────────────┘")
            System.Console.WriteLine()
            
            ' Option 3 - Combined Recovery
            System.Console.ForegroundColor = ConsoleColor.Magenta
            System.Console.WriteLine($"    ┌── [3] {GetIcon("🎯", "[C]")} SMART COMBINED RECOVERY ────────────────────────────────────────┐")
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("    │    File system scan + signature analysis (Best Results)               │")
            System.Console.WriteLine("    │    ✓ Maximum recovery rate for all scenarios                         │")
            System.Console.WriteLine("    │    ⏱ Estimated time: 45 minutes - 3 hours                           │")
            System.Console.ForegroundColor = ConsoleColor.Magenta
            System.Console.WriteLine("    └────────────────────────────────────────────────────────────────────────┘")
            System.Console.WriteLine()
            
            ' Option 4 - Advanced Settings
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine($"    ┌── [4] {GetIcon("⚙️", "[A]")} ADVANCED SETTINGS ─────────────────────────────────────────────┐")
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("    │    Custom recovery options and file type filtering                    │")
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine("    └────────────────────────────────────────────────────────────────────────┘")
            System.Console.WriteLine()
            
            ' Option 5 - Exit
            System.Console.ForegroundColor = ConsoleColor.Red
            System.Console.WriteLine($"    ┌── [5] {GetIcon("❌", "[X]")} EXIT PROGRAM ───────────────────────────────────────────────────┐")
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("    │    Close File Recall and return to system                             │")
            System.Console.ForegroundColor = ConsoleColor.Red
            System.Console.WriteLine("    └────────────────────────────────────────────────────────────────────────┘")
            System.Console.WriteLine()
            
            ' Professional input prompt
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.Write("    ► Enter your choice (1-5): ")
            System.Console.ForegroundColor = ConsoleColor.White
            
            Dim input As String = System.Console.ReadLine()
            Dim choice As Integer
            If Integer.TryParse(input, choice) AndAlso choice >= 1 AndAlso choice <= 5 Then
                Return choice
            Else
                ShowError("Invalid selection. Please enter a number between 1-5.")
                Threading.Thread.Sleep(2000)
                Return ShowMainMenu()
            End If
        End Function

        Public Shared Function SelectTargetLocation() As DriveSelectionResult
            System.Console.Clear()
            DrawBorder("Target Location Selection")
            
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine("   📍 Select the location where your lost files were stored:")
            
            ' Get available drives
            Dim drives As DriveInfo() = DriveInfo.GetDrives()
            Dim validDrives As New List(Of DriveInfo)
            
            System.Console.ForegroundColor = ConsoleColor.White
            Dim driveIndex As Integer = 1
            
            For Each drive As DriveInfo In drives
                If drive.DriveType = DriveType.Fixed OrElse drive.DriveType = DriveType.Removable Then
                    validDrives.Add(drive)
                    
                    System.Console.WriteLine($"   [{driveIndex}] 💽 Drive {drive.Name}")
                    System.Console.WriteLine($"       └─ Type: {GetDriveTypeDescription(drive.DriveType)}")
                    
                    If drive.IsReady Then
                        Dim totalGB As Double = drive.TotalSize / (1024.0 * 1024.0 * 1024.0)
                        Dim freeGB As Double = drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0)
                        System.Console.WriteLine($"       └─ Size: {totalGB:F1} GB ({freeGB:F1} GB free)")
                        System.Console.WriteLine($"       └─ Label: {If(String.IsNullOrEmpty(drive.VolumeLabel), "No label", drive.VolumeLabel)}")
                    Else
                        System.Console.WriteLine("       └─ Status: Not ready")
                    End If
                    
                    System.Console.WriteLine()
                    driveIndex += 1
                End If
            Next
            
            If validDrives.Count = 0 Then
                ShowError("No valid drives found for recovery.")
                Return New DriveSelectionResult With {.Success = False}
            End If
            
            System.Console.WriteLine("   [0] 🔙 Back to main menu")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.Write($"   Select drive (0-{validDrives.Count}): ")
            
            Dim input As String = System.Console.ReadLine()
            Dim choice As Integer
            
            If Integer.TryParse(input, choice) Then
                If choice = 0 Then
                    Return New DriveSelectionResult With {.Success = False, .GoBack = True}
                ElseIf choice > 0 AndAlso choice <= validDrives.Count Then
                    Dim selectedDrive As DriveInfo = validDrives(choice - 1)
                    
                    ' Convert drive letter to physical drive number (simplified)
                    Dim driveNumber As Integer = Asc(selectedDrive.Name.ToUpper()(0)) - Asc("C"c)
                    If selectedDrive.Name.ToUpper().StartsWith("C") Then driveNumber = 0
                    
                    Return New DriveSelectionResult With {
                        .Success = True,
                        .SelectedDrive = selectedDrive,
                        .PhysicalDriveNumber = driveNumber,
                        .DrivePath = selectedDrive.Name
                    }
                End If
            End If
            
            ShowError("Invalid selection. Please try again.")
            Threading.Thread.Sleep(2000)
            Return SelectTargetLocation()
        End Function

        Public Shared Function SelectFileTypes() As String()
            System.Console.Clear()
            DrawBorder("File Type Selection")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.WriteLine("   📄 Choose file types to recover (or select All):")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("   [1] 📸 Images (jpg, png, gif, bmp, tiff)")
            System.Console.WriteLine("   [2] 📝 Documents (pdf, doc, docx, xls, xlsx, ppt, pptx)")
            System.Console.WriteLine("   [3] 🎵 Audio Files (mp3, wav, flac)")
            System.Console.WriteLine("   [4] 🎬 Video Files (mp4, avi, mov, wmv)")
            System.Console.WriteLine("   [5] 📦 Archives (zip, rar, 7z)")
            System.Console.WriteLine("   [6] 💾 All File Types")
            System.Console.WriteLine("   [7] 🎯 Custom Selection")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.Write("   Enter your choice (1-7): ")
            
            Dim input As String = System.Console.ReadLine()
            Dim choice As Integer
            
            If Integer.TryParse(input, choice) Then
                Select Case choice
                    Case 1
                        Return {"jpg", "jpeg", "png", "gif", "bmp", "tiff", "tif"}
                    Case 2
                        Return {"pdf", "doc", "docx", "xls", "xlsx", "ppt", "pptx"}
                    Case 3
                        Return {"mp3", "wav", "flac"}
                    Case 4
                        Return {"mp4", "avi", "mov", "wmv"}
                    Case 5
                        Return {"zip", "rar", "7z"}
                    Case 6
                        Return Nothing ' All file types
                    Case 7
                        Return GetCustomFileTypes()
                End Select
            End If
            
            ShowError("Invalid selection. Please try again.")
            Threading.Thread.Sleep(2000)
            Return SelectFileTypes()
        End Function

        Private Shared Function GetCustomFileTypes() As String()
            System.Console.Clear()
            DrawBorder("Custom File Types")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine("   🎯 Enter file extensions separated by commas:")
            System.Console.WriteLine("   Example: jpg,png,pdf,docx,mp3")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.Write("   File extensions: ")
            
            Dim input As String = System.Console.ReadLine()
            If String.IsNullOrWhiteSpace(input) Then
                ShowError("No file types specified. Returning to selection menu.")
                Threading.Thread.Sleep(2000)
                Return SelectFileTypes()
            End If
            
            Dim extensions As String() = input.Split(","c).Select(Function(ext) ext.Trim().ToLower().TrimStart("."c)).Where(Function(ext) Not String.IsNullOrWhiteSpace(ext)).ToArray()
            
            If extensions.Length = 0 Then
                ShowError("No valid file extensions found. Returning to selection menu.")
                Threading.Thread.Sleep(2000)
                Return SelectFileTypes()
            End If
            
            Return extensions
        End Function

        Public Shared Function SelectOutputDirectory(defaultPath As String) As String
            System.Console.Clear()
            DrawBorder("Recovery Output Location")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.WriteLine("   📁 Where would you like to save recovered files?")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine("   ⚠️  Choose a different drive than the recovery source!")
            System.Console.WriteLine("   ⚠️  Ensure you have enough free space for recovered files.")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine($"   Default location: {defaultPath}")
            System.Console.WriteLine()
            System.Console.WriteLine("   [1] Use default location")
            System.Console.WriteLine("   [2] Choose custom location")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.Write("   Enter your choice (1-2): ")
            
            Dim choice As String = System.Console.ReadLine()
            
            If choice = "1" Then
                If Not Directory.Exists(defaultPath) Then
                    Try
                        Directory.CreateDirectory(defaultPath)
                    Catch ex As Exception
                        ShowError($"Cannot create default directory: {ex.Message}")
                        Threading.Thread.Sleep(3000)
                        Return SelectOutputDirectory(defaultPath)
                    End Try
                End If
                Return defaultPath
                
            ElseIf choice = "2" Then
                System.Console.Write("   Enter custom path: ")
                Dim customPath As String = System.Console.ReadLine()
                
                If String.IsNullOrWhiteSpace(customPath) Then
                    ShowError("Invalid path specified.")
                    Threading.Thread.Sleep(2000)
                    Return SelectOutputDirectory(defaultPath)
                End If
                
                Try
                    If Not Directory.Exists(customPath) Then
                        Directory.CreateDirectory(customPath)
                    End If
                    Return customPath
                Catch ex As Exception
                    ShowError($"Cannot access/create directory: {ex.Message}")
                    Threading.Thread.Sleep(3000)
                    Return SelectOutputDirectory(defaultPath)
                End Try
            Else
                ShowError("Invalid choice. Please select 1 or 2.")
                Threading.Thread.Sleep(2000)
                Return SelectOutputDirectory(defaultPath)
            End If
        End Function

        Public Shared Sub ShowRecoveryProgress(current As Integer, total As Integer, currentFile As String, Optional bytesProcessed As Long = 0)
            System.Console.SetCursorPosition(0, System.Console.CursorTop)
            
            Dim progressWidth As Integer = 50
            Dim progress As Double = If(total > 0, CDbl(current) / total, 0)
            Dim filledWidth As Integer = CInt(progress * progressWidth)
            
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.Write("   Progress: [")
            System.Console.Write(New String("█"c, filledWidth))
            System.Console.ForegroundColor = ConsoleColor.DarkGray
            System.Console.Write(New String("░"c, progressWidth - filledWidth))
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.Write($"] {progress:P1}")
            
            System.Console.WriteLine()
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine($"   Files: {current:N0} / {total:N0}")
            
            If bytesProcessed > 0 Then
                System.Console.WriteLine($"   Data processed: {FormatBytes(bytesProcessed)}")
            End If
            
            If Not String.IsNullOrEmpty(currentFile) Then
                System.Console.ForegroundColor = ConsoleColor.Cyan
                Dim displayFile As String = If(currentFile.Length > 60, "..." & currentFile.Substring(currentFile.Length - 57), currentFile)
                System.Console.WriteLine($"   Current: {displayFile}")
            End If
        End Sub

        Public Shared Sub ShowRecoveryResults(result As RecoveryEngine.RecoveryResult, outputPath As String)
            System.Console.Clear()
            SetConsoleColors()
            System.Console.WriteLine()
            
            ' Stunning completion header
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.WriteLine("    ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓")
            System.Console.WriteLine("    ┃                              RECOVERY COMPLETED!                               ┃")
            System.Console.WriteLine("    ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.WriteLine($"    {GetIcon("✅", "[✓]")} File recovery operation completed successfully!")
            System.Console.WriteLine()
            
            ' Beautiful statistics display
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.WriteLine("    ╔════════════════════════════════════════════════════════════════════════════╗")
            System.Console.WriteLine("    ║                               RECOVERY STATISTICS                             ║")
            System.Console.WriteLine("    ╚════════════════════════════════════════════════════════════════════════════╝")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine($"    📊 Files Found:          {result.TotalFilesFound:N0}")
            System.Console.WriteLine($"    💾 Data Recovered:       {FormatBytes(result.TotalBytesRecovered)}")
            System.Console.WriteLine($"    ⏱  Scan Duration:        {result.ScanDuration.TotalMinutes:F1} minutes")
            System.Console.WriteLine($"    🔧 Recovery Mode:        {result.ScanMode}")
            System.Console.WriteLine($"    ⚠️  Errors Encountered:   {result.ErrorCount:N0}")
            System.Console.WriteLine()
            
            If result.RecoveredFiles.Count > 0 Then
                System.Console.ForegroundColor = ConsoleColor.Green
                System.Console.WriteLine("    ╔════════════════════════════════════════════════════════════════════════════╗")
                System.Console.WriteLine("    ║                                OUTPUT LOCATION                             ║")
                System.Console.WriteLine("    ╚════════════════════════════════════════════════════════════════════════════╝")
                System.Console.WriteLine()
                System.Console.ForegroundColor = ConsoleColor.White
                System.Console.WriteLine($"    📁 Files saved to: {outputPath}")
                System.Console.WriteLine()
                
                ' Show file breakdown by category
                Dim categories = result.RecoveredFiles.Where(Function(f) f.IsSuccessful).
                    GroupBy(Function(f) f.FileInfo.FileCategory).
                    OrderByDescending(Function(g) g.Count())
                
                If categories.Any() Then
                    System.Console.ForegroundColor = ConsoleColor.Magenta
                    System.Console.WriteLine("    ╔════════════════════════════════════════════════════════════════════════════╗")
                    System.Console.WriteLine("    ║                               FILE BREAKDOWN                               ║")
                    System.Console.WriteLine("    ╚════════════════════════════════════════════════════════════════════════════╝")
                    System.Console.WriteLine()
                    
                    System.Console.ForegroundColor = ConsoleColor.White
                    For Each category In categories
                        Dim categoryName As String = category.Key.ToString()
                        Dim count As Integer = category.Count()
                        Dim totalSize As Long = category.Sum(Function(f) If(f.Data?.Length, 0))
                        System.Console.WriteLine($"    📈 {categoryName,-15}: {count,6:N0} files ({FormatBytes(totalSize)})")
                    Next
                End If
            Else
                System.Console.ForegroundColor = ConsoleColor.Yellow
                System.Console.WriteLine("    ╔════════════════════════════════════════════════════════════════════════════╗")
                System.Console.WriteLine("    ║                                   NO RESULTS                               ║")
                System.Console.WriteLine("    ╚════════════════════════════════════════════════════════════════════════════╝")
                System.Console.WriteLine()
                System.Console.WriteLine($"    {GetIcon("⚠️", "[!]")}  No files were successfully recovered from the selected drive.")
                System.Console.WriteLine()
                System.Console.ForegroundColor = ConsoleColor.Cyan
                System.Console.WriteLine("    💡 Recommendations:")
                System.Console.WriteLine("       • Try using a different recovery mode (Deep or Combined scan)")
                System.Console.WriteLine("       • Verify the drive contains the file types you're looking for")
                System.Console.WriteLine("       • Ensure the drive hasn't been overwritten extensively")
                System.Console.WriteLine("       • Consider professional data recovery services for critical data")
            End If
            
            System.Console.WriteLine()
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.WriteLine("    ╭─────────────────────────────────────────────────────────────────────────────╮")
            System.Console.WriteLine("    │                        Press any key to continue...                            │")
            System.Console.WriteLine("    ╰─────────────────────────────────────────────────────────────────────────────╯")
            
            System.Console.ResetColor()
            System.Console.ReadKey(True)
        End Sub

        Public Shared Sub ShowError(message As String)
            System.Console.ForegroundColor = ConsoleColor.Red
            System.Console.WriteLine()
            System.Console.WriteLine($"   ❌ Error: {message}")
            System.Console.WriteLine()
            System.Console.ForegroundColor = ConsoleColor.White
        End Sub

        Public Shared Sub ShowWarning(message As String)
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine()
            System.Console.WriteLine($"   ⚠️  Warning: {message}")
            System.Console.WriteLine()
            System.Console.ForegroundColor = ConsoleColor.White
        End Sub

        Public Shared Sub ShowInfo(message As String)
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.WriteLine()
            System.Console.WriteLine($"   ℹ️  {message}")
            System.Console.WriteLine()
            System.Console.ForegroundColor = ConsoleColor.White
        End Sub

        Public Shared Function ConfirmRecovery(driveInfo As DriveSelectionResult, fileTypes As String(), mode As String) As Boolean
            System.Console.Clear()
            DrawBorder("Confirm Recovery Settings")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine("   🔍 Please confirm your recovery settings:")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine($"   Target Drive: {driveInfo.DrivePath}")
            System.Console.WriteLine($"   Recovery Mode: {mode}")
            System.Console.WriteLine($"   File Types: {If(fileTypes Is Nothing, "All types", String.Join(", ", fileTypes))}")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Red
            System.Console.WriteLine("   ⚠️  IMPORTANT WARNINGS:")
            System.Console.WriteLine("   • This operation requires administrator privileges")
            System.Console.WriteLine("   • Recovery process may take significant time")
            System.Console.WriteLine("   • Do not interrupt the process once started")
            System.Console.WriteLine("   • Ensure target drive is not being used by other processes")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.Write("   Do you want to proceed? (y/n): ")
            
            Dim response As String = System.Console.ReadLine()?.ToLower()
            Return response = "y" OrElse response = "yes"
        End Function

        Private Shared Sub SetConsoleColors()
            System.Console.BackgroundColor = ConsoleColor.Black
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.Clear()
        End Sub

        Public Shared Sub DrawBorder(title As String)
            System.Console.ForegroundColor = ConsoleColor.Green
            
            ' Top border - reduced width for compact display
            System.Console.WriteLine(TopLeftCorner & New String(BorderChar(0), 70) & TopRightCorner)
            
            ' Title line
            Dim padding As Integer = (70 - title.Length) \ 2
            System.Console.WriteLine(VerticalChar & New String(" "c, padding) & title & New String(" "c, 70 - padding - title.Length) & VerticalChar)
            
            ' Bottom border
            System.Console.WriteLine(BottomLeftCorner & New String(BorderChar(0), 70) & BottomRightCorner)
            
            System.Console.ForegroundColor = ConsoleColor.White
        End Sub

        Private Shared Function GetDriveTypeDescription(driveType As DriveType) As String
            Select Case driveType
                Case DriveType.Fixed
                    Return "Fixed Hard Drive"
                Case DriveType.Removable
                    Return "Removable Drive"
                Case DriveType.Network
                    Return "Network Drive"
                Case DriveType.CDRom
                    Return "CD/DVD Drive"
                Case Else
                    Return "Unknown"
            End Select
        End Function

        Private Shared Function FormatBytes(bytes As Long) As String
            If bytes = 0 Then Return "0 B"
            
            Dim units As String() = {"B", "KB", "MB", "GB", "TB"}
            Dim unitIndex As Integer = 0
            Dim size As Double = bytes
            
            While size >= 1024 AndAlso unitIndex < units.Length - 1
                size /= 1024
                unitIndex += 1
            End While
            
            Return $"{size:F2} {units(unitIndex)}"
        End Function

        Public Class DriveSelectionResult
            Public Property Success As Boolean
            Public Property GoBack As Boolean
            Public Property SelectedDrive As DriveInfo
            Public Property PhysicalDriveNumber As Integer
            Public Property DrivePath As String
        End Class

    End Class

End Namespace
