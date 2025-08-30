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
            System.Console.WriteLine($"      {GetIcon("�", "[>]")}  Advanced Recovery Engine        ║  Raw disk sector-level analysis")
            System.Console.WriteLine($"      {GetIcon("💾", "[D]")}  Multi-Format Support           ║  30+ file signatures detected")
            System.Console.WriteLine($"      {GetIcon("🛡️", "[S]")}  NTFS Deep Analysis             ║  Master File Table parsing")
            System.Console.WriteLine($"      {GetIcon("�", "[?]")}  Intelligent Scanning            ║  Multiple recovery strategies")
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
            System.Console.WriteLine("    │                    Press any key to launch the Recovery Wizard              │")
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
                    
                    ' Add special indicator for USB/Removable drives
                    Dim driveIcon = If(drive.DriveType = DriveType.Removable, "🔌", "💽")
                    System.Console.WriteLine($"   [{driveIndex}] {driveIcon} Drive {drive.Name}")
                    System.Console.WriteLine($"       └─ Type: {GetDriveTypeDescription(drive.DriveType)}")
                    
                    If drive.DriveType = DriveType.Removable Then
                        System.Console.ForegroundColor = ConsoleColor.Yellow
                        System.Console.WriteLine($"       └─ ⚠️  USB/Removable drives may have limited recovery support")
                        System.Console.ForegroundColor = ConsoleColor.White
                    End If
                    
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

        Public Shared Function SelectSpecificFolder(selectedDrive As DriveInfo) As FolderSelectionResult
            System.Console.Clear()
            SetConsoleColors()
            System.Console.WriteLine()
            
            ' Beautiful folder selection header
            System.Console.ForegroundColor = ConsoleColor.Magenta
            System.Console.WriteLine("    ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓")
            System.Console.WriteLine("    ┃                           FOLDER TARGETING OPTIONS                             ┃")
            System.Console.WriteLine("    ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.WriteLine($"    Selected Drive: {selectedDrive.Name} ({selectedDrive.VolumeLabel})")
            System.Console.WriteLine("    Choose recovery scope for maximum efficiency:")
            System.Console.WriteLine()
            
            ' Option 1 - Scan Entire Drive
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.WriteLine($"    ┌── [1] {GetIcon("💾", "[D]")} SCAN ENTIRE DRIVE ─────────────────────────────────────────────┐")
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("    │    Comprehensive scan of all sectors on the selected drive               │")
            System.Console.WriteLine("    │    ✓ Finds all recoverable files regardless of original location        │")
            System.Console.WriteLine("    │    ⏱ Longer scan time but maximum file recovery potential               │")
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.WriteLine("    └──────────────────────────────────────────────────────────────────────────┘")
            System.Console.WriteLine()
            
            ' Option 2 - Target Specific Folder
            System.Console.ForegroundColor = ConsoleColor.Blue
            System.Console.WriteLine($"    ┌── [2] {GetIcon("📁", "[F]")} TARGET SPECIFIC FOLDER ──────────────────────────────────────────┐")
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("    │    Focus recovery on files from a specific folder location              │")
            System.Console.WriteLine("    │    ✓ Faster, more targeted recovery process                             │")
            System.Console.WriteLine("    │    ✓ Better organization and reduced false positives                    │")
            System.Console.ForegroundColor = ConsoleColor.Blue
            System.Console.WriteLine("    └──────────────────────────────────────────────────────────────────────────┘")
            System.Console.WriteLine()
            
            ' Option 3 - Common Folders
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine($"    ┌── [3] {GetIcon("⭐", "[*]")} COMMON FOLDERS ─────────────────────────────────────────────────┐")
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("    │    Quick selection from typical user folders                            │")
            System.Console.WriteLine("    │    📊 Desktop, Documents, Pictures, Downloads, Videos                    │")
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine("    └──────────────────────────────────────────────────────────────────────────┘")
            System.Console.WriteLine()
            
            ' Back option
            System.Console.ForegroundColor = ConsoleColor.Gray
            System.Console.WriteLine($"    ┌── [0] {GetIcon("↩️", "[B]")} BACK TO DRIVE SELECTION ─────────────────────────────────────────┐")
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("    │    Return to drive selection screen                                      │")
            System.Console.ForegroundColor = ConsoleColor.Gray
            System.Console.WriteLine("    └──────────────────────────────────────────────────────────────────────────┘")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.Write("    ► Enter your choice (0-3): ")
            System.Console.ForegroundColor = ConsoleColor.White
            
            Dim input As String = System.Console.ReadLine()
            Dim choice As Integer
            
            If Integer.TryParse(input, choice) Then
                Select Case choice
                    Case 0
                        Return New FolderSelectionResult With {.Success = False, .GoBack = True}
                    Case 1
                        Return New FolderSelectionResult With {
                            .Success = True,
                            .ScanEntireDrive = True,
                            .TargetPath = selectedDrive.Name,
                            .Description = "Full drive scan"
                        }
                    Case 2
                        Return SelectCustomFolder(selectedDrive)
                    Case 3
                        Return SelectCommonFolder(selectedDrive)
                End Select
            End If
            
            ShowError("Invalid selection. Please enter a number between 0-3.")
            Threading.Thread.Sleep(2000)
            Return SelectSpecificFolder(selectedDrive)
        End Function

        Private Shared Function SelectCustomFolder(selectedDrive As DriveInfo) As FolderSelectionResult
            System.Console.Clear()
            SetConsoleColors()
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Blue
            System.Console.WriteLine("    ╔════════════════════════════════════════════════════════════════════════════╗")
            System.Console.WriteLine("    ║                            CUSTOM FOLDER PATH                             ║")
            System.Console.WriteLine("    ╚════════════════════════════════════════════════════════════════════════════╝")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine($"    📁 Enter the original folder path where your files were located:")
            System.Console.WriteLine()
            System.Console.WriteLine("    Examples:")
            System.Console.WriteLine($"    • {selectedDrive.Name}Users\\YourName\\Desktop")
            System.Console.WriteLine($"    • {selectedDrive.Name}Users\\YourName\\Documents\\Projects")
            System.Console.WriteLine($"    • {selectedDrive.Name}ImportantFiles")
            System.Console.WriteLine($"    • {selectedDrive.Name}Work\\Presentations")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine("    💡 Tips:")
            System.Console.WriteLine("    • Use the exact path where files were originally stored")
            System.Console.WriteLine("    • Include subfolders if you want to scan recursively")
            System.Console.WriteLine("    • Leave blank to scan the entire drive")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.Write($"    ► Folder path (or press Enter for full drive): ")
            
            Dim customPath As String = System.Console.ReadLine()?.Trim()
            
            If String.IsNullOrEmpty(customPath) Then
                Return New FolderSelectionResult With {
                    .Success = True,
                    .ScanEntireDrive = True,
                    .TargetPath = selectedDrive.Name,
                    .Description = "Full drive scan (no path specified)"
                }
            Else
                ' Ensure path starts with drive letter
                If Not customPath.StartsWith(selectedDrive.Name) Then
                    customPath = Path.Combine(selectedDrive.Name, customPath.TrimStart("\"c))
                End If
                
                Return New FolderSelectionResult With {
                    .Success = True,
                    .ScanEntireDrive = False,
                    .TargetPath = customPath,
                    .Description = $"Target folder: {customPath}"
                }
            End If
        End Function

        Private Shared Function SelectCommonFolder(selectedDrive As DriveInfo) As FolderSelectionResult
            System.Console.Clear()
            SetConsoleColors()
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine("    ╔════════════════════════════════════════════════════════════════════════════╗")
            System.Console.WriteLine("    ║                              COMMON FOLDERS                                ║")
            System.Console.WriteLine("    ╚════════════════════════════════════════════════════════════════════════════╝")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("    Select from commonly used folder locations:")
            System.Console.WriteLine()
            
            Dim commonFolders As New List(Of (Index As Integer, Icon As String, Name As String, Path As String, Description As String)) From {
                (1, "🖥️", "Desktop", $"{selectedDrive.Name}Users\*\Desktop", "Desktop files and shortcuts"),
                (2, "📄", "Documents", $"{selectedDrive.Name}Users\*\Documents", "Word docs, PDFs, text files"),
                (3, "🖼️", "Pictures", $"{selectedDrive.Name}Users\*\Pictures", "Photos, images, graphics"),
                (4, "📥", "Downloads", $"{selectedDrive.Name}Users\*\Downloads", "Downloaded files and installers"),
                (5, "🎬", "Videos", $"{selectedDrive.Name}Users\*\Videos", "Video files and recordings"),
                (6, "🎵", "Music", $"{selectedDrive.Name}Users\*\Music", "Audio files and music"),
                (7, "💼", "Program Files", $"{selectedDrive.Name}Program Files", "Installed applications"),
                (8, "🗂️", "Root Directory", $"{selectedDrive.Name}", "Files in drive root")
            }
            
            For Each folder In commonFolders
                System.Console.WriteLine($"    [{folder.Index}] {GetIcon(folder.Icon, $"[{folder.Index}]")} {folder.Name}")
                System.Console.WriteLine($"        └─ {folder.Description}")
                System.Console.WriteLine($"        └─ Path: {folder.Path}")
                System.Console.WriteLine()
            Next
            
            System.Console.WriteLine("    [0] 🔙 Back to folder options")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.Write($"    ► Select folder (0-8): ")
            System.Console.ForegroundColor = ConsoleColor.White
            
            Dim input As String = System.Console.ReadLine()
            Dim choice As Integer
            
            If Integer.TryParse(input, choice) Then
                If choice = 0 Then
                    Return New FolderSelectionResult With {.Success = False, .GoBack = True}
                ElseIf choice >= 1 AndAlso choice <= commonFolders.Count Then
                    Dim selectedFolder = commonFolders(choice - 1)
                    Return New FolderSelectionResult With {
                        .Success = True,
                        .ScanEntireDrive = False,
                        .TargetPath = selectedFolder.Path,
                        .Description = $"{selectedFolder.Name}: {selectedFolder.Description}"
                    }
                End If
            End If
            
            ShowError("Invalid selection. Please try again.")
            Threading.Thread.Sleep(2000)
            Return SelectCommonFolder(selectedDrive)
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
            
            ' Show helpful information when no files found
            If result.TotalFilesFound = 0 Then
                System.Console.ForegroundColor = ConsoleColor.Yellow
                System.Console.WriteLine("    ╔════════════════════════════════════════════════════════════════════════════╗")
                System.Console.WriteLine("    ║                          WHY NO FILES WERE FOUND?                         ║")
                System.Console.WriteLine("    ╚════════════════════════════════════════════════════════════════════════════╝")
                System.Console.WriteLine()
                System.Console.ForegroundColor = ConsoleColor.White
                System.Console.WriteLine("    📝 Common reasons for zero results:")
                System.Console.WriteLine()
                System.Console.WriteLine("    💾 MODERN STORAGE: SSDs use TRIM commands that immediately erase deleted data")
                System.Console.WriteLine("    ⚡ QUICK OVERWRITE: Files may be overwritten within seconds of deletion")
                System.Console.WriteLine("    🗂️  RECYCLE BIN: Files might be in Recycle Bin, not actually 'deleted'")
                System.Console.WriteLine("    🔄 FILE SYSTEM: NTFS may reuse space immediately for small files")
                System.Console.WriteLine("    📁 WRONG PATH: The specified folder might not match the actual file location")
                System.Console.WriteLine()
                System.Console.ForegroundColor = ConsoleColor.Cyan
                System.Console.WriteLine("    💡 SUGGESTIONS TO IMPROVE RECOVERY:")
                System.Console.WriteLine("    • Try 'Scan Entire Drive' instead of folder targeting")
                System.Console.WriteLine("    • Check Recycle Bin first before using recovery tools")
                System.Console.WriteLine("    • Use recovery immediately after deletion (within minutes)")
                System.Console.WriteLine("    • For SSDs, disable TRIM temporarily during recovery")
                System.Console.WriteLine("    • Try 'Deep Signature Scan' mode for maximum coverage")
                System.Console.WriteLine()
            End If
            
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

        Public Shared Function ConfirmRecovery(driveInfo As DriveSelectionResult, fileTypes As String(), mode As String, Optional folderInfo As FolderSelectionResult = Nothing) As Boolean
            System.Console.Clear()
            SetConsoleColors()
            System.Console.WriteLine()
            
            ' Beautiful confirmation header
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.WriteLine("    ╔════════════════════════════════════════════════════════════════════════════╗")
            System.Console.WriteLine("    ║                            CONFIRM RECOVERY SETTINGS                          ║")
            System.Console.WriteLine("    ╚════════════════════════════════════════════════════════════════════════════╝")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.WriteLine("    🔍 Please review your recovery configuration:")
            System.Console.WriteLine()
            
            ' Recovery settings display
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("    ┌─────────────────── RECOVERY CONFIGURATION ───────────────────┐")
            System.Console.WriteLine($"    │  Target Drive:     {driveInfo.DrivePath,-35} │")
            System.Console.WriteLine($"    │  Recovery Mode:    {mode,-35} │")
            If folderInfo IsNot Nothing Then
                Dim scopeText = If(folderInfo.ScanEntireDrive, "Full Drive Scan", folderInfo.Description)
                System.Console.WriteLine($"    │  Scan Scope:       {scopeText,-35} │")
                If Not folderInfo.ScanEntireDrive Then
                    System.Console.WriteLine($"    │  Target Path:      {folderInfo.TargetPath,-35} │")
                End If
            End If
            Dim fileTypeText = If(fileTypes Is Nothing, "All supported file types", String.Join(", ", fileTypes))
            System.Console.WriteLine($"    │  File Types:       {fileTypeText,-35} │")
            System.Console.WriteLine("    └────────────────────────────────────────────────────────────────┘")
            System.Console.WriteLine()
            
            ' Important warnings
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine("    ┌─────────────────── IMPORTANT WARNINGS ───────────────────────┐")
            System.Console.ForegroundColor = ConsoleColor.Red
            System.Console.WriteLine("    │  ⚠️  Administrator privileges required for disk access          │")
            System.Console.WriteLine("    │  ⚠️  Recovery process may take significant time to complete     │")
            System.Console.WriteLine("    │  ⚠️  Do not interrupt the process once recovery has started    │")
            System.Console.WriteLine("    │  ⚠️  Ensure target drive is not actively being used            │")
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine("    └────────────────────────────────────────────────────────────────┘")
            System.Console.WriteLine()
            
            ' Professional confirmation prompt
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.WriteLine("    ╭─────────────────────────────────────────────────────────────────╮")
            System.Console.WriteLine("    │                    Ready to begin recovery?                        │")
            System.Console.WriteLine("    ╰─────────────────────────────────────────────────────────────────╯")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.Write("    ► Proceed with recovery? (y/n): ")
            System.Console.ForegroundColor = ConsoleColor.White
            
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
                    Return "Fixed Hard Drive (HDD/SSD)"
                Case DriveType.Removable
                    Return "USB/Removable Drive"
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

        Public Class FolderSelectionResult
            Public Property Success As Boolean
            Public Property GoBack As Boolean
            Public Property ScanEntireDrive As Boolean
            Public Property TargetPath As String
            Public Property Description As String
        End Class

    End Class

End Namespace
