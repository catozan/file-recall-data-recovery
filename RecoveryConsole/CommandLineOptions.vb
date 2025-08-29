Imports System.ComponentModel
Imports CommandLine

''' <summary>
''' Command line options for the data recovery tool
''' </summary>
Public Class CommandLineOptions

    <[Option]("d"c, "drive", Required:=True, HelpText:="Physical drive number to scan (0 = first drive)")>
    Public Property DriveNumber As Integer

    <[Option]("m"c, "mode", Required:=False, Default:=RecoveryMode.Combined, HelpText:="Recovery mode: FileSystemOnly, SignatureOnly, Combined, DeepScan")>
    Public Property Mode As RecoveryMode

    <[Option]("o"c, "output", Required:=True, HelpText:="Output directory for recovered files")>
    Public Property OutputDirectory As String

    <[Option]("e"c, "extensions", Required:=False, HelpText:="Target file extensions (comma-separated, e.g., 'jpg,png,docx')")>
    Public Property Extensions As String

    <[Option]("s"c, "maxsize", Required:=False, Default:=107374182400L, HelpText:="Maximum scan size in bytes (default: 100GB)")>
    Public Property MaxScanSize As Long

    <[Option]("v"c, "verbose", Required:=False, Default:=False, HelpText:="Enable verbose logging")>
    Public Property Verbose As Boolean

    <[Option]("r"c, "report", Required:=False, HelpText:="Generate detailed recovery report to specified file")>
    Public Property ReportFile As String

    <[Option]("t"c, "threads", Required:=False, Default:=4, HelpText:="Number of processing threads (default: 4)")>
    Public Property ThreadCount As Integer

    <[Option]("c"c, "confidence", Required:=False, Default:=0.5, HelpText:="Minimum confidence level for file recovery (0.0-1.0)")>
    Public Property MinConfidence As Double

    Public Function GetExtensionsArray() As String()
        If String.IsNullOrWhiteSpace(Extensions) Then
            Return Nothing
        End If
        
        Return Extensions.Split(","c).Select(Function(ext) ext.Trim().TrimStart("."c)).ToArray()
    End Function

End Class

Public Enum RecoveryMode
    <Description("Use file system metadata only (MFT/FAT records)")>
    FileSystemOnly
    
    <Description("Use file signature detection only")>
    SignatureOnly
    
    <Description("Combine file system and signature methods")>
    Combined
    
    <Description("Deep sector-by-sector analysis")>
    DeepScan
End Enum
