namespace DocumentAnalysisInterface;

/// <summary>Where this app's glyph sources live: the directory glyph declarations are found in and new ones written to, and the namespace new source files declare.</summary>
public sealed record GlyphSourceLocation(string Directory, string Namespace)
{
    /// <summary>The path a new file declaring <paramref name="typeName"/> alone is written to.</summary>
    public string PathFor(string typeName) => Path.Combine(Directory, typeName + ".cs");
}
