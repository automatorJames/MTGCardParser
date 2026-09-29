namespace Glyphotype.GlyphPrimitives;

/// <summary>
/// Base for everything a tokenizer can produce for a span of source text: <see cref="Glyph"/>s,
/// which captured a recognized construct and its property graph, and
/// <see cref="UnmatchedString"/>, which captured nothing and exists only to fill the
/// gaps between recognized constructs. Code that analyzes recognized constructs (e.g. TypeRegexPage)
/// should target <see cref="Glyph"/> directly - filtering a CaptureUnit sequence with
/// OfType&lt;Glyph&gt;() excludes unmatched spans without needing to special-case the type.
/// </summary>
public abstract class CaptureUnit
{
    /// <summary>Run within a bounded window, matches exactly that window, whatever its length.</summary>
    static readonly Regex _wholeWindow = new(".+", RegexOptions.Singleline);

    /// <summary>
    /// A <see cref="CaptureContext"/> spanning exactly <paramref name="length"/> characters of
    /// <paramref name="sourceText"/> from <paramref name="index"/> - for a unit the Tokenizer constructs directly
    /// (e.g. <see cref="UnmatchedString"/>) rather than matching with a graph. <paramref name="rootNode"/> is
    /// shared by every instance of the unit, just as a matched Glyph type's graph nodes are.
    /// </summary>
    protected static CaptureContext CreateSpanContext(GlyphNode rootNode, string sourceText, int index, int length) =>
        new(rootNode, _wholeWindow.Match(sourceText, index, length), sourceText);

    /// <summary>A root node for a directly constructed unit, with its (lazily built) children forced up front, since one instance is shared across threads.</summary>
    protected static TNode CreateSharedRootNode<TNode>(TNode node) where TNode : GlyphNode
    {
        _ = node.Children;
        return node;
    }

    public CaptureContext CaptureContext { get; set; }

    public string CaptureValue =>
        CaptureContext.FullMatch;

    public string JsonDebug =>
        CaptureContext.RootCaptureTrace.JsonDebug ?? "";

    Type _type;
    public Type Type
    {
        get
        {
            if (_type is null)
                _type = GetType();

            return _type;
        }
    }

    public override string ToString() => $"{Type.Name}: \"{CaptureContext.ToString()}\"";
}
