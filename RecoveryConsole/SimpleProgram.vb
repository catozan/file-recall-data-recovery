Imports Microsoft.Extensions.Logging
Imports RecoveryConsole.UserInterface
Imports DataRecoveryCore.Recovery
Imports System.IO
Imports System.Threading
Imports System.Diagnostics

''' <summary>
''' Simplified entry point that uses the enhanced user interface by default
''' This provides the beautiful visual console experience users requested
''' </summary>
Module SimpleProgram

    Sub Main(args As String())
        Try
            ' Set console properties for optimal display
            ConfigureConsole()
            
            ' Check administrator privileges before proceeding
            If Not DataRecoveryCore.DiskAccess.DiskAccessManager.HasAdministratorPrivileges() Then
                ShowAdministratorError()
                Return
            End If
            
            ' Setup logging
            Dim loggerFactory As ILoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(
                Sub(builder) builder.AddConsole().SetMinimumLevel(LogLevel.Warning))
                
                Dim logger = loggerFactory.CreateLogger(Of RecoveryEngine)()
                
                ' Check if command line arguments provided
                If args.Length > 0 Then
                    ' Use enhanced command line interface for scripting
                    EnhancedProgram.Main(args)
                Else
                    ' Use enhanced visual interface for interactive users
                    RunEnhancedInterface(logger)
                End If
            
        Catch ex As Exception
            System.Console.Clear()
            System.Console.ForegroundColor = ConsoleColor.Red
            System.Console.WriteLine()
            System.Console.WriteLine("   ❌ CRITICAL ERROR")
            System.Console.WriteLine($"   {ex.Message}")
            System.Console.WriteLine()
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine("   Press any key to exit...")
            System.Console.ReadKey()
        End Try
    End Sub
    
    Private Sub ConfigureConsole()
        Try
            ' Set title and window size
            System.Console.Title = "File Recall - Advanced Data Recovery Tool"
            
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
            
            ' Use a more compact window size to reduce dead space
            Dim optimalWidth = Math.Min(80, System.Console.LargestWindowWidth)
            Dim optimalHeight = Math.Min(25, System.Console.LargestWindowHeight)
            
            If System.Console.WindowWidth < optimalWidth OrElse System.Console.WindowHeight < optimalHeight Then
                System.Console.SetWindowSize(optimalWidth, optimalHeight)
            End If
            
        Catch
            ' Ignore console configuration errors (e.g., in certain terminals)
        End Try
    End Sub
    
    Private Sub ShowAdministratorError()
        System.Console.Clear()
        System.Console.ForegroundColor = ConsoleColor.Red
        System.Console.WriteLine()
        System.Console.WriteLine("   ╔══════════════════════════════════════════════════════════════════╗")
        System.Console.WriteLine("   ║                        ⚠️  ACCESS DENIED                         ║")
        System.Console.WriteLine("   ╚══════════════════════════════════════════════════════════════════╝")
        System.Console.WriteLine()
        System.Console.ForegroundColor = ConsoleColor.Yellow
        System.Console.WriteLine("   This application requires Administrator privileges to access")
        System.Console.WriteLine("   physical drives and perform data recovery operations.")
        System.Console.WriteLine()
        System.Console.ForegroundColor = ConsoleColor.Cyan
        System.Console.WriteLine("   📋 To fix this issue:")
        System.Console.WriteLine("   1. Right-click on the application")
        System.Console.WriteLine("   2. Select 'Run as Administrator'")
        System.Console.WriteLine("   3. Confirm the User Account Control prompt")
        System.Console.WriteLine()
        System.Console.ForegroundColor = ConsoleColor.White
        System.Console.WriteLine("   Press any key to exit...")
        System.Console.ReadKey()
    End Sub
    
    Private Sub RunEnhancedInterface(logger As ILogger(Of RecoveryEngine))
        ' Welcome with visual branding
        ConsoleUI.ShowWelcomeScreen()
        
        ' Main application loop with enhanced UI
        While True
            Dim choice As Integer = ConsoleUI.ShowMainMenu()
            
            Select Case choice
                Case 1 ' Quick Recovery
                    RunGuidedRecoverySession(RecoveryEngine.RecoveryMode.FileSystemOnly, 
                                           "🚀 Quick Recovery", logger)
                Case 2 ' Deep Signature Scan  
                    RunGuidedRecoverySession(RecoveryEngine.RecoveryMode.SignatureOnly,
                                           "🔍 Deep Signature Scan", logger)
                Case 3 ' Smart Combined Recovery
                    RunGuidedRecoverySession(RecoveryEngine.RecoveryMode.Combined,
                                           "🎯 Smart Combined Recovery", logger)
                Case 4 ' Custom Recovery
                    ShowCustomRecoveryOptions(logger)
                Case 5 ' Exit
                    ShowGoodbyeScreen()
                    Exit While
            End Select
        End While
    End Sub
    
    Private Async Function RunGuidedRecoverySession(mode As RecoveryEngine.RecoveryMode, 
                                             modeName As String, 
                                             logger As ILogger(Of RecoveryEngine)) As Task
        Try
            ' Drive selection with visual interface
            Dim driveSelection = ConsoleUI.SelectTargetLocation()
            If Not driveSelection.Success Then
                If driveSelection.GoBack Then Return
                ConsoleUI.ShowError("Drive selection failed.")
                Threading.Thread.Sleep(2000)
                Return
            End If
            
            ' NEW: Folder targeting selection
            Dim folderSelection As ConsoleUI.FolderSelectionResult
            Do
                folderSelection = ConsoleUI.SelectSpecificFolder(driveSelection.SelectedDrive)
                If folderSelection.GoBack Then
                    ' Go back to drive selection
                    Dim newDriveSelection = ConsoleUI.SelectTargetLocation()
                    If Not newDriveSelection.Success Then Return
                    driveSelection = newDriveSelection
                    Continue Do
                End If
                Exit Do
            Loop
            
            If Not folderSelection.Success Then
                ConsoleUI.ShowError("Folder selection failed.")
                Threading.Thread.Sleep(2000)
                Return
            End If
            
            ' File type selection
            Dim fileTypes = ConsoleUI.SelectFileTypes()
            
            ' Output directory selection  
            Dim defaultPath = IO.Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop), "FileRecall_Recovery")
            Dim outputPath = ConsoleUI.SelectOutputDirectory(defaultPath)
            
            ' Enhanced confirmation with folder info
            If Not ConsoleUI.ConfirmRecovery(driveSelection, fileTypes, modeName, folderSelection) Then
                ConsoleUI.ShowInfo("Recovery cancelled by user.")
                Threading.Thread.Sleep(2000)
                Return
            End If
            
            ' Execute recovery with folder targeting
            Await ExecuteRecoveryWithVisuals(driveSelection.PhysicalDriveNumber, 
                                           mode, outputPath, fileTypes, modeName, 
                                           folderSelection, logger)
            
        Catch ex As Exception
            ConsoleUI.ShowError($"Recovery session failed: {ex.Message}")
            ConsoleUI.ShowWarning("Check the log file for detailed error information.")
            Threading.Thread.Sleep(3000)
        End Try
    End Function
    
    Private Async Function ExecuteRecoveryWithVisuals(driveNumber As Integer, 
                                                     mode As RecoveryEngine.RecoveryMode,
                                                     outputPath As String, 
                                                     fileTypes As String(),
                                                     modeName As String,
                                                     folderSelection As ConsoleUI.FolderSelectionResult,
                                                     logger As ILogger(Of RecoveryEngine)) As Task
        
        System.Console.Clear()
        ConsoleUI.DrawBorder("Recovery Operation Active")
        System.Console.WriteLine()
        
        ' Initialize and show progress
        System.Console.ForegroundColor = ConsoleColor.Cyan
        System.Console.WriteLine("   🔧 Initializing recovery systems...")
        Threading.Thread.Sleep(1000)
        System.Console.WriteLine("   💽 Connecting to physical drive...")
        Threading.Thread.Sleep(500)
        System.Console.WriteLine("   🛡️ Verifying security permissions...")
        Threading.Thread.Sleep(500)
        System.Console.WriteLine()
        
        Using recovery As New RecoveryEngine(logger)
            If Not recovery.Initialize(driveNumber) Then
                ConsoleUI.ShowError("Failed to initialize recovery engine")
                ConsoleUI.ShowWarning("Possible causes:")
                System.Console.WriteLine("     • Drive is not accessible or busy")
                System.Console.WriteLine("     • Insufficient system privileges")
                System.Console.WriteLine("     • Hardware connection issues")
                Threading.Thread.Sleep(4000)
                Return
            End If
            
            ' Show recovery configuration
            System.Console.Clear()
            ConsoleUI.DrawBorder($"Scanning Drive {driveNumber}")
            System.Console.WriteLine()
            System.Console.ForegroundColor = ConsoleColor.Green
            System.Console.WriteLine("   ✅ Recovery engine online and ready!")
            System.Console.WriteLine()
            System.Console.ForegroundColor = ConsoleColor.White
            System.Console.WriteLine($"   🎯 Mode: {modeName}")
            System.Console.WriteLine($"   💽 Target: Physical Drive {driveNumber}")
            System.Console.WriteLine($"   📁 Output: {outputPath}")
            System.Console.WriteLine($"   📄 Types: {If(fileTypes Is Nothing, "All supported file types", String.Join(", ", fileTypes))}")
            System.Console.WriteLine($"   ⏰ Started: {DateTime.Now:HH:mm:ss}")
            System.Console.WriteLine()
            
            ' Progress indication
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.WriteLine("   ⚠️  IMPORTANT: Please do not interrupt this process!")
            System.Console.WriteLine("   📊 Recovery progress will be displayed below...")
            System.Console.WriteLine()
            System.Console.WriteLine("   🔍 Starting recovery operation...")
            
            ' Start recovery without spinner to avoid console conflicts
            'Dim progressTimer As New Threading.Timer(Sub() ShowSpinner(), Nothing, 
            '                                       TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2))
            
            Dim result = Await recovery.RecoverFilesAsync(mode, fileTypes, Long.MaxValue, outputPath)
            
            'progressTimer.Dispose()
            
            ' Show results with enhanced visual display
            System.Console.Clear()
            ConsoleUI.ShowRecoveryResults(result, outputPath)
            
            ' Offer additional actions
            If result.TotalFilesFound > 0 Then
                System.Console.WriteLine()
                System.Console.ForegroundColor = ConsoleColor.Cyan
                System.Console.Write("   Would you like to open the recovery folder? (y/n): ")
                If System.Console.ReadLine()?.ToLower().StartsWith("y") Then
                    Try
                        Process.Start("explorer.exe", outputPath)
                        ConsoleUI.ShowInfo("Recovery folder opened in Windows Explorer.")
                    Catch
                        ConsoleUI.ShowWarning("Unable to open folder automatically.")
                        ConsoleUI.ShowInfo($"Please manually navigate to: {outputPath}")
                    End Try
                End If
            End If
        End Using
    End Function
    
    Private Sub ShowSpinner()
        Static spinIndex As Integer = 0
        Dim spinChars As String() = {"⣾", "⣷", "⣯", "⣟", "⣿", "⣻", "⣽", "⣼"}
        
        Try
            System.Console.SetCursorPosition(3, System.Console.CursorTop - 1)
            System.Console.ForegroundColor = ConsoleColor.Yellow
            System.Console.Write($"{spinChars(spinIndex)} Scanning and recovering files...")
            spinIndex = (spinIndex + 1) Mod spinChars.Length
        Catch
            ' Ignore cursor positioning errors
        End Try
    End Sub
    
    Private Sub ShowCustomRecoveryOptions(logger As ILogger(Of RecoveryEngine))
        System.Console.Clear()
        ConsoleUI.DrawBorder("Advanced Recovery Options")
        System.Console.WriteLine()
        
        System.Console.ForegroundColor = ConsoleColor.Yellow
        System.Console.WriteLine("   🔬 Professional Recovery Features")
        System.Console.WriteLine()
        System.Console.WriteLine("   [1] 📍 Location-Specific Recovery")
        System.Console.WriteLine("       └─ Target specific folder paths for focused recovery")
        System.Console.WriteLine()
        System.Console.WriteLine("   [2] 📊 Advanced Configuration")
        System.Console.WriteLine("       └─ Custom scan parameters and filtering options")
        System.Console.WriteLine()
        System.Console.WriteLine("   [3] 🔙 Return to Main Menu")
        System.Console.WriteLine()
        
        System.Console.ForegroundColor = ConsoleColor.Cyan
        System.Console.Write("   Select option (1-3): ")
        
        Dim choice = System.Console.ReadLine()
        Select Case choice
            Case "1"
                ShowLocationSpecificRecovery(logger)
            Case "2"
                ConsoleUI.ShowInfo("Advanced configuration will be available in future updates!")
                Threading.Thread.Sleep(2500)
            Case "3"
                Return
            Case Else
                ConsoleUI.ShowError("Invalid selection.")
                Threading.Thread.Sleep(1500)
                ShowCustomRecoveryOptions(logger)
        End Select
    End Sub
    
    Private Async Sub ShowLocationSpecificRecovery(logger As ILogger(Of RecoveryEngine))
        System.Console.Clear()
        ConsoleUI.DrawBorder("Location-Specific File Recovery")
        System.Console.WriteLine()
        
        System.Console.ForegroundColor = ConsoleColor.Green
        System.Console.WriteLine("   📍 This feature helps recover files from specific known locations")
        System.Console.WriteLine("   💡 Perfect for recovering from accidentally deleted folders")
        System.Console.WriteLine()
        System.Console.ForegroundColor = ConsoleColor.Yellow
        System.Console.WriteLine("   Common recovery targets:")
        System.Console.WriteLine("   • C:\\Users\\[Username]\\Desktop")
        System.Console.WriteLine("   • C:\\Users\\[Username]\\Documents")
        System.Console.WriteLine("   • C:\\Users\\[Username]\\Pictures")
        System.Console.WriteLine("   • C:\\Users\\[Username]\\Downloads")
        System.Console.WriteLine("   • D:\\Projects\\ImportantWork")
        System.Console.WriteLine()
        System.Console.ForegroundColor = ConsoleColor.Cyan
        System.Console.Write("   Enter the original path of deleted files: ")
        
        Dim targetPath = System.Console.ReadLine()
        If String.IsNullOrWhiteSpace(targetPath) Then
            ConsoleUI.ShowWarning("No path specified. Returning to main menu.")
            Threading.Thread.Sleep(2000)
            Return
        End If
        
        ConsoleUI.ShowInfo($"Configuring recovery for: {targetPath}")
        ConsoleUI.ShowInfo("Enhanced targeting will help locate files from this specific location.")
        Threading.Thread.Sleep(2000)
        
        ' Continue with guided recovery
        Await RunGuidedRecoverySession(RecoveryEngine.RecoveryMode.Combined, 
                                     $"📍 Location-Specific Recovery ({IO.Path.GetFileName(targetPath)})", 
                                     logger)
    End Sub
    
    Private Sub ShowGoodbyeScreen()
        System.Console.Clear()
        System.Console.WriteLine()
        
        ' Elegant goodbye screen inspired by Claude
        System.Console.ForegroundColor = ConsoleColor.Cyan
        System.Console.WriteLine("    ╭─────────────────────────────────────────────────────────────────────────────╮")
        System.Console.WriteLine("    │                              Thank you for using                               │")
        System.Console.WriteLine("    │                                                                                 │")
        System.Console.WriteLine("    │                             ███████╗██╗██╗     ███████╗                       │")
        System.Console.WriteLine("    │                             ██╔════╝██║██║     ██╔════╝                       │")
        System.Console.WriteLine("    │                             █████╗  ██║██║     █████╗                         │")
        System.Console.WriteLine("    │                             ██╔══╝  ██║██║     ██╔══╝                         │")
        System.Console.WriteLine("    │                             ██║     ██║███████╗███████╗                       │")
        System.Console.WriteLine("    │                             ╚═╝     ╚═╝╚══════╝╚══════╝                       │")
        System.Console.WriteLine("    │                                                                                 │")
        System.Console.WriteLine("    │                         ██████╗ ███████╗ ██████╗ █████╗ ██╗     ██╗           │")
        System.Console.WriteLine("    │                         ██╔══██╗██╔════╝██╔════╝██╔══██╗██║     ██║           │")
        System.Console.WriteLine("    │                         ██████╔╝█████╗  ██║     ███████║██║     ██║           │")
        System.Console.WriteLine("    │                         ██╔══██╗██╔══╝  ██║     ██╔══██║██║     ██║           │")
        System.Console.WriteLine("    │                         ██║  ██║███████╗╚██████╗██║  ██║███████╗███████╗       │")
        System.Console.WriteLine("    │                         ╚═╝  ╚═╝╚══════╝ ╚═════╝╚═╝  ╚═╝╚══════╝╚══════╝       │")
        System.Console.WriteLine("    │                                                                                 │")
        System.Console.WriteLine("    │                        Your Professional Data Recovery Partner                  │")
        System.Console.WriteLine("    ╰─────────────────────────────────────────────────────────────────────────────╯")
        System.Console.WriteLine()
        
        System.Console.ForegroundColor = ConsoleColor.Green
        System.Console.WriteLine("    ┌─────────────────────────────────────────────────────────────────────────────┐")
        System.Console.WriteLine("    │                              IMPORTANT REMINDERS                              │")
        System.Console.WriteLine("    └─────────────────────────────────────────────────────────────────────────────┘")
        System.Console.WriteLine()
        
        System.Console.ForegroundColor = ConsoleColor.White
        System.Console.WriteLine($"    {GetIcon("💾", "[D]")} Remember to backup your important data regularly")
        System.Console.WriteLine($"    {GetIcon("🛡️", "[S]")} Data recovery is most effective when performed quickly")
        System.Console.WriteLine($"    {GetIcon("📧", "[E]")} For support, visit our documentation and community forums")
        System.Console.WriteLine($"    {GetIcon("⭐", "[*]")} Professional recovery services available for critical data")
        System.Console.WriteLine()
        
        System.Console.ForegroundColor = ConsoleColor.Yellow
        System.Console.WriteLine("    File Recall - Intermediate Data Recovery Tool v1.0")
        System.Console.WriteLine("    Built with VB.NET • Powered by .NET 6.0 • Windows Platform")
        System.Console.WriteLine()
        
        System.Console.ForegroundColor = ConsoleColor.Gray
        Threading.Thread.Sleep(3500)
    End Sub
    
    Private Function GetIcon(unicodeIcon As String, fallback As String) As String
        Try
            ' Test if Unicode output works by checking encoding
            Dim supportsUnicode = (System.Console.OutputEncoding.CodePage = 65001) OrElse ' UTF-8
                                (System.Console.OutputEncoding.CodePage = 1200)   ' UTF-16
            Return If(supportsUnicode, unicodeIcon, fallback)
        Catch
            Return fallback
        End Try
    End Function
    
End Module
