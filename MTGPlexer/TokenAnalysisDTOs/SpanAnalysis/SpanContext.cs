namespace MTGPlexer.TokenAnalysisDTOs.SpanAnalysis;

/// <summary>
/// Internal state bag used during the recursive build process.
/// Handles coordinate offsets, path prefixes, and the naming chain for collapsed nodes.
/// </summary>
internal record SpanContext(
    string FullText,
    string PathPrefix,
    int AbsoluteOffset = 0,
    IReadOnlyList<string>? NameChain = null)
{
    public IReadOnlyList<string> CurrentNameChain => NameChain ?? Array.Empty<string>();

    public SpanContext WithOffset(int addedOffset) => this with { AbsoluteOffset = AbsoluteOffset + addedOffset };

    public SpanContext WithPath(string path) => this with { PathPrefix = path };

    public SpanContext PushName(string name) =>
        this with { NameChain = new List<string>(CurrentNameChain) { name } };

    public SpanContext ClearNameChain() => this with { NameChain = null };

    /// <summary>
    /// Helper to resolve palettes for generic objects like ManyOf or DynamicCapture.
    /// </summary>
    public Palette GetPalette(object owner)
    {
        var colorAttr = owner.GetType().GetCustomAttribute<ColorAttribute>();
        return colorAttr != null
            ? DeterministicPalette.GetStaticPalette(colorAttr.Color)
            : DeterministicPalette.GetFixedRainbowPalette(0);
    }
}