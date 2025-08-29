Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DataRecoveryCore.FileSignatures
Imports Microsoft.Extensions.Logging

<TestClass>
Public Class FileSignatureAnalyzerTests

    Private _logger As ILogger(Of FileSignatureAnalyzer)
    Private _loggerFactory As ILoggerFactory

    <TestInitialize>
    Public Sub Setup()
        _loggerFactory = LoggerFactory.Create(Sub(builder) builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
        _logger = _loggerFactory.CreateLogger(Of FileSignatureAnalyzer)
    End Sub

    <TestCleanup>
    Public Sub Cleanup()
        _loggerFactory?.Dispose()
    End Sub

    <TestMethod>
    Public Sub TestJpegSignatureDetection()
        ' Arrange
        Dim analyzer As New FileSignatureAnalyzer(_logger)
        Dim jpegHeader As Byte() = {&HFF, &HD8, &HFF, &HE0, &H0, &H10, &H4A, &H46, &H49, &H46}
        
        ' Act
        Dim results = analyzer.AnalyzeDataBlock(jpegHeader, 0)
        
        ' Assert
        Assert.IsTrue(results.Count > 0)
        Assert.AreEqual("jpg", results(0).Signature.Extension)
    End Sub

    <TestMethod>
    Public Sub TestPngSignatureDetection()
        ' Arrange
        Dim analyzer As New FileSignatureAnalyzer(_logger)
        Dim pngHeader As Byte() = {&H89, &H50, &H4E, &H47, &HD, &HA, &H1A, &HA, &H0, &H0, &H0, &HD, &H49, &H48, &H44, &H52}
        
        ' Act
        Dim results = analyzer.AnalyzeDataBlock(pngHeader, 0)
        
        ' Assert
        Assert.IsTrue(results.Count > 0)
        Assert.AreEqual("png", results(0).Signature.Extension)
    End Sub

    <TestMethod>
    Public Sub TestPdfSignatureDetection()
        ' Arrange
        Dim analyzer As New FileSignatureAnalyzer(_logger)
        Dim pdfHeader As Byte() = {&H25, &H50, &H44, &H46, &H2D, &H31, &H2E, &H34} ' "%PDF-1.4"
        
        ' Act
        Dim results = analyzer.AnalyzeDataBlock(pdfHeader, 0)
        
        ' Assert
        Assert.IsTrue(results.Count > 0)
        Assert.AreEqual("pdf", results(0).Signature.Extension)
    End Sub

    <TestMethod>
    Public Sub TestMultipleSignaturesInBlock()
        ' Arrange
        Dim analyzer As New FileSignatureAnalyzer(_logger)
        Dim combinedData As New List(Of Byte)
        
        ' Add JPEG signature
        combinedData.AddRange({&HFF, &HD8, &HFF, &HE0})
        combinedData.AddRange(New Byte(1020) {}) ' Padding
        
        ' Add PNG signature
        combinedData.AddRange({&H89, &H50, &H4E, &H47, &HD, &HA, &H1A, &HA})
        
        ' Act
        Dim results = analyzer.AnalyzeDataBlock(combinedData.ToArray(), 0)
        
        ' Assert
        Assert.IsTrue(results.Count >= 2)
        Assert.IsTrue(results.Any(Function(r) r.Signature.Extension = "jpg"))
        Assert.IsTrue(results.Any(Function(r) r.Signature.Extension = "png"))
    End Sub

    <TestMethod>
    Public Sub TestExtensionFiltering()
        ' Arrange
        Dim analyzer As New FileSignatureAnalyzer(_logger)
        Dim targetExtensions As String() = {"jpg", "png"}
        
        ' Act
        Dim filteredSignatures = analyzer.GetSignaturesByExtension(targetExtensions)
        
        ' Assert
        Assert.IsTrue(filteredSignatures.Count > 0)
        Assert.IsTrue(filteredSignatures.All(Function(s) targetExtensions.Contains(s.Extension)))
    End Sub

    <TestMethod>
    Public Sub TestCategoryFiltering()
        ' Arrange
        Dim analyzer As New FileSignatureAnalyzer(_logger)
        
        ' Act
        Dim imageSignatures = analyzer.GetSignaturesByCategory(FileCategory.Image)
        
        ' Assert
        Assert.IsTrue(imageSignatures.Count > 0)
        Assert.IsTrue(imageSignatures.All(Function(s) s.Category = FileCategory.Image))
    End Sub

End Class
