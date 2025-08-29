Module UnicodeTest
    Sub TestUnicodeSupport()
        ' Test UTF-8 encoding configuration
        Try
            System.Console.OutputEncoding = System.Text.Encoding.UTF8
            System.Console.WriteLine("UTF-8 encoding set successfully")
        Catch ex As Exception
            System.Console.WriteLine("Failed to set UTF-8 encoding: " & ex.Message)
        End Try
        
        ' Test Unicode detection
        Dim codePage = System.Console.OutputEncoding.CodePage
        System.Console.WriteLine("Current CodePage: " & codePage)
        
        Dim supportsUnicode = (codePage = 65001 OrElse codePage = 1200)
        System.Console.WriteLine("Supports Unicode: " & supportsUnicode)
        
        ' Test emoji display
        System.Console.WriteLine()
        System.Console.WriteLine("Testing emoji display:")
        System.Console.WriteLine("🔄 Loading emoji (should show spinning arrow)")
        System.Console.WriteLine("💾 Disk emoji (should show floppy disk)")
        System.Console.WriteLine("🛡️ Shield emoji (should show shield)")
        System.Console.WriteLine("📁 Folder emoji (should show folder)")
        System.Console.WriteLine("🚀 Rocket emoji (should show rocket)")
        
        System.Console.WriteLine()
        System.Console.WriteLine("ASCII Fallbacks:")
        System.Console.WriteLine("[*] Loading (ASCII fallback)")
        System.Console.WriteLine("[D] Disk (ASCII fallback)")
        System.Console.WriteLine("[S] Shield (ASCII fallback)")
        System.Console.WriteLine("[F] Folder (ASCII fallback)")
        System.Console.WriteLine("[>] Rocket (ASCII fallback)")
        
        System.Console.WriteLine()
        System.Console.WriteLine("Press any key to continue...")
        System.Console.ReadKey(True)
    End Sub
End Module
