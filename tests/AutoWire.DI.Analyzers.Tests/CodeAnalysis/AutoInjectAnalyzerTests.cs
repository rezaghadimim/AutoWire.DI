using AutoWire.DI.Analyzers.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace AutoWire.DI.Analyzers.Tests.CodeAnalysis;

public class AutoInjectAnalyzerTests : CSharpAnalyzerTest<AutoInjectAnalyzer, DefaultVerifier>
{
    [Fact]
    public async Task DoesNotReportOnClassWithAutoInject()
    {
        // Arrange: Class marked with the AutoInject attribute
        var code =  @"
        using System;

        namespace YourNamespace
        {
            [AutoInject]
            public class MyService
            {
            }
        }";

        // No diagnostics expected since AutoInject is present
        var expected = Array.Empty<DiagnosticResult>();

        // Act & Assert
        await CSharpAnalyzerVerifier<AutoInjectAnalyzer, DefaultVerifier>
            .VerifyAnalyzerAsync(code, expected);
    }
}
