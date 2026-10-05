using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Glyphotype.Tests.Infrastructure;

/// <summary>Compiles glyph source the way a glyph project would - with Glyphotype's namespaces as global usings, and warnings as errors - and loads it.</summary>
public static class SourceCompiler
{
    const string _globalUsings = """
        global using System;
        global using System.Collections.Generic;
        global using System.Linq;
        global using Glyphotype;
        global using Glyphotype.Attributes;
        global using Glyphotype.Attributes.Quantifiers;
        global using Glyphotype.BackReferences;
        global using Glyphotype.GlyphPrimitives;
        global using Glyphotype.NibHelpers;
        """;

    public static Assembly Compile(params string[] sources)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.Latest);

        // Not this test assembly, whose own test grammar would otherwise clash with a compiled copy of it.
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator)
            .Where(x => !string.Equals(x, typeof(SourceCompiler).Assembly.Location, StringComparison.OrdinalIgnoreCase))
            .Append(typeof(Glyph).Assembly.Location)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(x => MetadataReference.CreateFromFile(x));

        var compilation = CSharpCompilation.Create(
            $"Compiled{Guid.NewGuid():N}",
            sources.Prepend(_globalUsings).Select(x => CSharpSyntaxTree.ParseText(x, parseOptions)),
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);
        var problems = result.Diagnostics.Where(x => x.Severity >= DiagnosticSeverity.Warning).ToList();

        Assert.True(result.Success && problems.Count == 0, string.Join("\n", problems) + "\n\n" + string.Join("\n\n", sources));

        return Assembly.Load(stream.ToArray());
    }
}
