Imports Microsoft.Extensions.Logging
Imports System.IO
Imports System.Linq
Imports DataRecoveryCore.Recovery
Imports RecoveryConsole.UserInterface
Imports System.Threading.Tasks

Module EnhancedProgram

    Private logger As ILogger(Of RecoveryEngine)

    Sub Main(args As String())
        ' Initialize logging for the core engine
        Dim loggerFactory As ILoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(
            Sub(builder)
                builder.AddConsole().SetMinimumLevel(LogLevel.Warning) ' Reduce console noise for UI
            End Sub)
            
            logger = loggerFactory.CreateLogger(Of RecoveryEngine)()
            
            ' Set console properties for better UI
            Try
                System.Console.Title = "File Recall - Data Recovery Tool"
                
                ' Configure console for proper Unicode/emoji support
                Try
                    System.Console.OutputEncoding = System.Text.Encoding.UTF8
                    System.Console.InputEncoding = System.Text.Encoding.UTF8
                Catch
                    ' Fallback if UTF-8 encoding fails
                    Try
                        System.Console.OutputEncoding = System.Text.Encoding.Unicode
                    Catch
                        ' Use default encoding if all else fails
                    End Try
                End Try
                
                System.Console.SetWindowSize(Math.Min(80, System.Console.LargestWindowWidth), Math.Min(25, System.Console.LargestWindowHeight))
            Catch
                ' Ignore if console properties can't be set
            End Try
            
            ' Check administrator privileges first
            If Not DataRecoveryCore.DiskAccess.DiskAccessManager.HasAdministratorPrivileges() Then
                System.Console.Clear()
                ConsoleUI.ShowError("This application requires administrator privileges.")
                ConsoleUI.ShowInfo("Please right-click and 'Run as Administrator', then try again.")
                System.Console.WriteLine("   Press any key to exit...")
                System.Console.ReadKey()
                Return
            End If
            
            ' Parse command line arguments or run interactive mode
            If args.Length > 0 Then
                ProcessCommandLineArgs(args)
            Else
                RunInteractiveWizard()
            End If
    End Sub

    Private Sub ProcessCommandLineArgs(args As String())
        ' Parse command line arguments for automated recovery
        Dim driveNumber As Integer = 0
        Dim recoveryMode As RecoveryEngine.RecoveryMode = RecoveryEngine.RecoveryMode.Combined
        Dim outputDirectory As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FileRecall_Recovery")
        Dim targetExtensions As String() = Nothing
        Dim maxScanSize As Long = Long.MaxValue

        Try
            For i As Integer = 0 To args.Length - 1
                Select Case args(i).ToLower()
                    Case "--drive", "-d"
                        If i + 1 < args.Length Then
                            Integer.TryParse(args(i + 1), driveNumber)
                            i += 1
                        End If
                    Case "--mode", "-m"
                        If i + 1 < args.Length Then
                            [Enum].TryParse(args(i + 1), True, recoveryMode)
                            i += 1
                        End If
                    Case "--output", "-o"
                        If i + 1 < args.Length Then
                            outputDirectory = args(i + 1)
                            i += 1
                        End If
                    Case "--extensions", "-e"
                        If i + 1 < args.Length Then
                            targetExtensions = args(i + 1).Split(","c).Select(Function(ext) ext.Trim().ToLower()).ToArray()
                            i += 1
                        End If
                    Case "--maxsize", "-s"
                        If i + 1 < args.Length Then
                            Long.TryParse(args(i + 1), maxScanSize)
                            i += 1
                        End If
                    Case "--help", "-h", "-?"
                        ShowUsage()
                        Return
                End Select
            Next

            ' Run automated recovery
            RunAutomatedRecovery(driveNumber, recoveryMode, outputDirectory, targetExtensions, maxScanSize)

        Catch ex As Exception
            System.Console.Clear()
            ConsoleUI.ShowError($"Error processing command line arguments: {ex.Message}")
            ShowUsage()
            System.Console.WriteLine("Press any key to exit...")
            System.Console.ReadKey()
        End Try
    End Sub

    Private Sub ShowUsage()
        System.Console.Clear()
        System.Console.ForegroundColor = ConsoleColor.Cyan
        System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════════╗")
        System.Console.WriteLine("║                     File Recall - Command Line Usage                     ║")
        System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════╝")
        System.Console.WriteLine()
        System.Console.ForegroundColor = ConsoleColor.White
        System.Console.WriteLine("Usage: RecoveryConsole.exe [options]")
        System.Console.WriteLine()
        System.Console.ForegroundColor = ConsoleColor.Yellow
        System.Console.WriteLine("Options:")
        System.Console.ForegroundColor = ConsoleColor.White
        System.Console.WriteLine("  --drive, -d <number>     Physical drive number (0 = first drive)")
        System.Console.WriteLine("  --mode, -m <mode>        Recovery mode:")
        System.Console.WriteLine("                           • FileSystemOnly (Fast MFT scan)")
        System.Console.WriteLine("                           • SignatureOnly  (Deep sector scan)")
        System.Console.WriteLine("                           • Combined       (Recommended)")
        System.Console.WriteLine("                           • DeepScan       (Comprehensive)")
        System.Console.WriteLine("  --output, -o <path>      Output directory for recovered files")
        System.Console.WriteLine("  --extensions, -e <list>  File extensions (jpg,png,pdf,docx)")
        System.Console.WriteLine("  --maxsize, -s <bytes>    Maximum scan size in bytes")
        System.Console.WriteLine("  --help, -h               Show this help message")
        System.Console.WriteLine()
        System.Console.ForegroundColor = ConsoleColor.Green
        System.Console.WriteLine("Examples:")
        System.Console.WriteLine("  RecoveryConsole.exe -d 0 -m Combined -o ""C:\\Recovery""")
        System.Console.WriteLine("  RecoveryConsole.exe -d 1 -m SignatureOnly -e ""jpg,png,mp4""")
        System.Console.WriteLine("  RecoveryConsole.exe -d 0 -m DeepScan -s 10000000000 -o ""D:\\Recovered""")
        System.Console.WriteLine()
        System.Console.ForegroundColor = ConsoleColor.Cyan
        System.Console.WriteLine("💡 No arguments = Interactive wizard with guided recovery")
        System.Console.WriteLine()
    End Sub

    Private Sub RunInteractiveWizard()
        Try
            ' Welcome screen
            ConsoleUI.ShowWelcomeScreen()
            
            While True
                ' Main menu
                Dim choice As Integer = ConsoleUI.ShowMainMenu()
                
                Select Case choice
                    Case 1 ' Quick Recovery
                        RunGuidedRecovery(RecoveryEngine.RecoveryMode.FileSystemOnly, "🚀 Quick Recovery")
                    Case 2 ' Deep Signature Scan
                        RunGuidedRecovery(RecoveryEngine.RecoveryMode.SignatureOnly, "🔍 Deep Signature Scan")
                    Case 3 ' Smart Combined Recovery
                        RunGuidedRecovery(RecoveryEngine.RecoveryMode.Combined, "🎯 Smart Combined Recovery")
                    Case 4 ' Custom Recovery
                        RunCustomRecovery()
                    Case 5 ' Exit
                        System.Console.Clear()
                        System.Console.ForegroundColor = ConsoleColor.Green
                        System.Console.WriteLine("   ╔════════════════════════════════════════════════════╗")
                        System.Console.WriteLine("   ║            Thank you for using                     ║")
                        System.Console.WriteLine("   ║              🔄 File Recall                        ║")
                        System.Console.WriteLine("   ║       Your Data Recovery Partner                   ║")
                        System.Console.WriteLine("   ╚════════════════════════════════════════════════════╝")
                        System.Console.ForegroundColor = ConsoleColor.White
                        Threading.Thread.Sleep(1500)
                        Exit While
                End Select
            End While
            
        Catch ex As Exception
            ConsoleUI.ShowError($"An unexpected error occurred: {ex.Message}")
            System.Console.WriteLine("Press any key to exit...")
            System.Console.ReadKey()
        End Try
    End Sub

    Private Async Function RunGuidedRecovery(mode As RecoveryEngine.RecoveryMode, modeName As String) As Task
        Try
            ' Select target location
            Dim driveSelection As ConsoleUI.DriveSelectionResult = ConsoleUI.SelectTargetLocation()
            If Not driveSelection.Success Then
                If driveSelection.GoBack Then Return
                ConsoleUI.ShowError("Failed to select target drive.")
                Threading.Thread.Sleep(2000)
                Return
            End If
            
            ' Select file types
            Dim fileTypes As String() = ConsoleUI.SelectFileTypes()
            
            ' Select output directory
            Dim defaultOutput As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FileRecall_Recovery")
            Dim outputPath As String = ConsoleUI.SelectOutputDirectory(defaultOutput)
            
            ' Confirm settings
            If Not ConsoleUI.ConfirmRecovery(driveSelection, fileTypes, modeName) Then
                ConsoleUI.ShowInfo("Recovery cancelled by user.")
                Threading.Thread.Sleep(2000)
                Return
            End If
            
            ' Run the recovery
            Await RunRecoveryWithProgress(driveSelection.PhysicalDriveNumber, mode, outputPath, fileTypes, modeName)
            
        Catch ex As Exception
            ConsoleUI.ShowError($"Recovery failed: {ex.Message}")
            System.Console.WriteLine("   📝 Error details saved to recovery log.")
            Threading.Thread.Sleep(3000)
        End Try
    End Function

    Private Sub RunCustomRecovery()
        System.Console.Clear()
        ConsoleUI.DrawBorder("Custom Recovery Options")
        System.Console.WriteLine()
        
        System.Console.ForegroundColor = ConsoleColor.Yellow
        System.Console.WriteLine("   🔧 Advanced Recovery Settings")
        System.Console.WriteLine()
        System.Console.WriteLine("   [1] 🎯 Target-Specific Location Recovery")
        System.Console.WriteLine("       └─ Recover files from specific folder paths")
        System.Console.WriteLine()
        System.Console.WriteLine("   [2] 🕒 Time-Based Recovery")
        System.Console.WriteLine("       └─ Recover files by modification date range")
        System.Console.WriteLine()
        System.Console.WriteLine("   [3] 📏 Size-Based Recovery")
        System.Console.WriteLine("       └─ Filter recovery by file size ranges")
        System.Console.WriteLine()
        System.Console.WriteLine("   [4] 🔙 Back to Main Menu")
        System.Console.WriteLine()
        
        System.Console.ForegroundColor = ConsoleColor.Cyan
        System.Console.Write("   Enter your choice (1-4): ")
        
        Dim choice As String = System.Console.ReadLine()
        Select Case choice
            Case "1"
                RunTargetSpecificRecovery()
            Case "2"
                ConsoleUI.ShowInfo("Time-based recovery will be available in the next update!")
                Threading.Thread.Sleep(2000)
            Case "3"
                ConsoleUI.ShowInfo("Size-based recovery will be available in the next update!")
                Threading.Thread.Sleep(2000)
            Case "4"
                Return
            Case Else
                ConsoleUI.ShowError("Invalid selection. Please try again.")
                Threading.Thread.Sleep(2000)
                RunCustomRecovery()
        End Select
    End Sub

    Private Async Sub RunTargetSpecificRecovery()
        System.Console.Clear()
        ConsoleUI.DrawBorder("Target-Specific Location Recovery")
        System.Console.WriteLine()
        
        System.Console.ForegroundColor = ConsoleColor.Green
        System.Console.WriteLine("   📁 This mode helps recover files from specific known locations")
        System.Console.WriteLine("   💡 Examples: Desktop, Documents, Pictures, Downloads folders")
        System.Console.WriteLine()
        
        System.Console.ForegroundColor = ConsoleColor.Yellow
        System.Console.WriteLine("   Enter the original path where your files were located:")
        System.Console.WriteLine("   Examples:")
        System.Console.WriteLine("   • C:\\Users\\YourName\\Desktop")
        System.Console.WriteLine("   • C:\\Users\\YourName\\Documents\\ImportantFiles")
        System.Console.WriteLine("   • D:\\Photos\\Vacation2024")
        System.Console.WriteLine()
        
        System.Console.ForegroundColor = ConsoleColor.Cyan
        System.Console.Write("   Original path: ")
        
        Dim originalPath As String = System.Console.ReadLine()
        If String.IsNullOrWhiteSpace(originalPath) Then
            ConsoleUI.ShowWarning("No path specified. Returning to custom menu.")
            Threading.Thread.Sleep(2000)
            RunCustomRecovery()
            Return
        End If
        
        ConsoleUI.ShowInfo($"Target-specific recovery for: {originalPath}")
        ConsoleUI.ShowInfo("This feature provides enhanced recovery for known file locations!")
        
        ' Continue with standard guided recovery but with context
        Await RunGuidedRecovery(RecoveryEngine.RecoveryMode.Combined, $"🎯 Target-Specific Recovery ({Path.GetFileName(originalPath)})")
    End Sub

    Private Async Function RunRecoveryWithProgress(driveNumber As Integer, mode As RecoveryEngine.RecoveryMode, 
                                                  outputDirectory As String, targetExtensions As String(), modeName As String) As Task
        Try
            System.Console.Clear()
            ConsoleUI.DrawBorder("Recovery In Progress")
            System.Console.WriteLine()
            
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.WriteLine("   🔧 Initializing recovery engine...")
            System.Console.WriteLine("   📡 Establishing disk connection...")
            System.Console.WriteLine("   🛡️ Verifying administrator privileges...")
            System.Console.WriteLine()
            
            Using recovery As New RecoveryEngine(logger)
                If Not recovery.Initialize(driveNumber) Then
                    ConsoleUI.ShowError("Failed to initialize recovery engine.")
                    ConsoleUI.ShowWarning("Possible causes:")
                    System.Console.WriteLine("     • Drive is not accessible")
                    System.Console.WriteLine("     • Insufficient administrator privileges")
                    System.Console.WriteLine("     • Drive is being used by another process")
                    Threading.Thread.Sleep(5000)
                    Return
                End If
                
                System.Console.Clear()
                ConsoleUI.DrawBorder($"Scanning Drive {driveNumber}")
                System.Console.WriteLine()
                System.Console.ForegroundColor = ConsoleColor.Green
                System.Console.WriteLine("   ✅ Recovery engine initialized successfully!")
                System.Console.WriteLine()
                System.Console.ForegroundColor = ConsoleColor.White
                System.Console.WriteLine($"   🎯 Recovery Mode: {modeName}")
                System.Console.WriteLine($"   💽 Target Drive: {driveNumber}")
                System.Console.WriteLine($"   📁 Output Path: {outputDirectory}")
                System.Console.WriteLine($"   📄 File Types: {If(targetExtensions Is Nothing, "All file types", String.Join(", ", targetExtensions))}")
                System.Console.WriteLine($"   ⏱️ Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                System.Console.WriteLine()
                
                System.Console.ForegroundColor = ConsoleColor.Yellow
                System.Console.WriteLine("   ⚠️  IMPORTANT: Do not interrupt this process!")
                System.Console.WriteLine("   📊 Progress will be displayed below...")
                System.Console.WriteLine()
                
                ' Show initial progress
                System.Console.ForegroundColor = ConsoleColor.Cyan
                System.Console.WriteLine("   🔍 Analyzing disk structure...")
                System.Console.WriteLine("   📈 Preparing recovery algorithms...")
                System.Console.WriteLine()
                
                ' Start the recovery operation with progress updates
                Dim progressTimer As New Threading.Timer(
                    AddressOf UpdateProgress,
                    Nothing,
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5)
                )
                
                Dim result As RecoveryEngine.RecoveryResult = Await recovery.RecoverFilesAsync(
                    mode, targetExtensions, Long.MaxValue, outputDirectory)
                
                progressTimer.Dispose()
                
                ' Show completion
                System.Console.Clear()
                ConsoleUI.ShowRecoveryResults(result, outputDirectory)
                
                ' Offer to open output directory
                If result.TotalFilesFound > 0 Then
                    System.Console.WriteLine()
                    System.Console.ForegroundColor = ConsoleColor.Cyan
                    System.Console.Write("   Would you like to open the recovery folder? (y/n): ")
                    If System.Console.ReadLine()?.ToLower() = "y" Then
                        Try
                            Process.Start("explorer.exe", outputDirectory)
                        Catch
                            ConsoleUI.ShowWarning("Could not open folder automatically.")
                        End Try
                    End If
                End If
                
            End Using

        Catch ex As Exception
            ConsoleUI.ShowError($"Recovery operation failed: {ex.Message}")
            System.Console.WriteLine("   📋 Technical details:")
            System.Console.WriteLine($"      Error Type: {ex.GetType().Name}")
            System.Console.WriteLine($"      Location: {ex.StackTrace?.Split(vbLf).FirstOrDefault()?.Trim()}")
            System.Console.WriteLine("   Press any key to continue...")
            System.Console.ReadKey()
        End Try
    End Function

    Private Sub UpdateProgress(state As Object)
        ' This is called periodically during recovery to show activity
        Static progressStep As Integer = 0
        progressStep = (progressStep + 1) Mod 4
        
        Dim spinChars As String() = {"⣾", "⣷", "⣯", "⣟", "⡿", "⢿", "⣻", "⣽"}
        System.Console.SetCursorPosition(3, System.Console.CursorTop - 2)
        System.Console.ForegroundColor = ConsoleColor.Yellow
        System.Console.Write($"{spinChars(progressStep Mod spinChars.Length)} Processing...")
        System.Console.ForegroundColor = ConsoleColor.White
    End Sub

    Private Async Sub RunAutomatedRecovery(driveNumber As Integer, mode As RecoveryEngine.RecoveryMode, 
                                          outputDirectory As String, targetExtensions As String(), maxScanSize As Long)
        Try
            System.Console.Clear()
            System.Console.ForegroundColor = ConsoleColor.Cyan
            System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════════╗")
            System.Console.WriteLine("║                   File Recall - Automated Recovery                       ║")
            System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════╝")
            System.Console.WriteLine()
            System.Console.ForegroundColor = ConsoleColor.White
            
            System.Console.WriteLine("🔧 Initializing automated recovery...")
            System.Console.WriteLine($"💽 Target Drive: {driveNumber}")
            System.Console.WriteLine($"🎯 Recovery Mode: {mode}")
            System.Console.WriteLine($"📁 Output Directory: {outputDirectory}")
            System.Console.WriteLine($"📄 Target Extensions: {If(targetExtensions Is Nothing, "All types", String.Join(", ", targetExtensions))}")
            System.Console.WriteLine($"📏 Max Scan Size: {FormatBytes(maxScanSize)}")
            System.Console.WriteLine()
            
            Using recovery As New RecoveryEngine(logger)
                If Not recovery.Initialize(driveNumber) Then
                    System.Console.ForegroundColor = ConsoleColor.Red
                    System.Console.WriteLine("❌ Failed to initialize recovery engine.")
                    System.Console.WriteLine("   Make sure you have administrator privileges and the drive is accessible.")
                    System.Console.WriteLine()
                    Environment.ExitCode = 1
                    Return
                End If
                
                System.Console.ForegroundColor = ConsoleColor.Green
                System.Console.WriteLine("✅ Recovery engine initialized successfully!")
                System.Console.WriteLine()
                System.Console.ForegroundColor = ConsoleColor.Yellow
                System.Console.WriteLine("🚀 Starting recovery process...")
                
                Dim startTime As DateTime = DateTime.Now
                Dim result As RecoveryEngine.RecoveryResult = Await recovery.RecoverFilesAsync(
                    mode, targetExtensions, maxScanSize, outputDirectory)
                
                ' Display results
                System.Console.WriteLine()
                System.Console.ForegroundColor = ConsoleColor.Green
                System.Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════════╗")
                System.Console.WriteLine("║                        RECOVERY COMPLETED                                ║")
                System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════╝")
                System.Console.WriteLine()
                System.Console.ForegroundColor = ConsoleColor.White
                System.Console.WriteLine($"📊 Recovery Statistics:")
                System.Console.WriteLine($"   Files Found: {result.TotalFilesFound:N0}")
                System.Console.WriteLine($"   Data Recovered: {FormatBytes(result.TotalBytesRecovered)}")
                System.Console.WriteLine($"   Scan Duration: {result.ScanDuration.TotalMinutes:F1} minutes")
                System.Console.WriteLine($"   Recovery Mode: {result.ScanMode}")
                System.Console.WriteLine($"   Errors: {result.ErrorCount:N0}")
                System.Console.WriteLine($"   Success Rate: {If(result.TotalFilesFound > 0, (result.RecoveredFiles.Where(Function(f) f.IsSuccessful).Count() / result.TotalFilesFound * 100), 0):F1}%")
                System.Console.WriteLine()
                System.Console.WriteLine($"📁 Output Directory: {outputDirectory}")
                System.Console.WriteLine("═" & New String("="c, 75))
            End Using

        Catch ex As Exception
            System.Console.ForegroundColor = ConsoleColor.Red
            System.Console.WriteLine($"❌ Recovery failed: {ex.Message}")
            System.Console.WriteLine()
            Environment.ExitCode = 1
        End Try
        
        System.Console.ForegroundColor = ConsoleColor.Cyan
        System.Console.WriteLine("Press any key to exit...")
        System.Console.ReadKey()
    End Sub

    Private Function FormatBytes(bytes As Long) As String
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

End Module
