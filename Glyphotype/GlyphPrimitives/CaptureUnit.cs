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
