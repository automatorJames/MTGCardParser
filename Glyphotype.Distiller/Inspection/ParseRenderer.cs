namespace Glyphotype.Distiller.Inspection;

/// <summary>
/// Renders tokenized text as one line of plain text, for reading rather than display: each match as
/// <c>⟦Glyph: text⟧</c>, each unmatched span as <c>«text»</c>, clause breaks as themselves. Nested, a match
/// shows its captures too - <c>⟦Property: …⟧</c> for a glyph, <c>⟦Property→Glyph: …⟧</c> for what a dynamic
/// resolved to, <c>⟦Property=text⟧</c> for a terminal (vocabulary, bool, number).
/// </summary>
public static class ParseRenderer
{
    public static string Render(ProcessedLine line, bool nested) =>
        Render(line.Glyphs, nested);

    public static string Render(IEnumerable<CaptureUnit> units, bool nested) =>
        string.Join(" ", units.Select(x => Render(x, nested)));

    public static string Render(CaptureUnit unit, bool nested)
    {
        var root = unit.CaptureContext.RootCaptureTrace;

        if (root.IsUnmatchedString)
            return $"«{unit.CaptureValue.Trim()}»";

        if (root.IsClauseBreak)
            return unit.CaptureValue.Trim();

        return $"⟦{unit.Type.Name}: {(nested ? RenderInner(root) : unit.CaptureValue).Trim()}⟧";
    }

    /// <summary><paramref name="trace"/>'s own text with its captures rendered in place.</summary>
    public static string RenderInner(CaptureTrace trace) =>
        string.Concat(CaptureTraceWalker.GetSegments(trace).Select(x => x.Child is null ? x.Text : RenderCapture(x.Child)));

    /// <summary>A capture in brackets - with any whitespace it captured at its edges (a bool's pattern can include a space) kept outside them, where it reads.</summary>
    static string RenderCapture(CaptureTrace trace)
    {
        var text = trace.IsTerminal ? trace.CaptureValue : RenderInner(trace);
        var leading = text[..^text.TrimStart().Length];
        var trailing = text[text.TrimEnd().Length..];

        if (trace.IsTerminal)
            return $"{leading}⟦{trace.Name}={text.Trim()}⟧{trailing}";

        var resolved = trace.SourceNode is DynamicGlyphNode ? $"→{trace.ResolvedNodeType?.Name}" : "";

        return $"{leading}⟦{trace.Name}{resolved}: {text.Trim()}⟧{trailing}";
    }

    /// <summary>
    /// The type a trace captured: the glyph a dynamic resolved to, else the type its node stands for - a glyph,
    /// an enum, a primitive.
    /// </summary>
    public static Type CapturedType(CaptureTrace trace) =>
        trace.SourceNode is DynamicGlyphNode ? trace.ResolvedNodeType : trace.SourceNode?.Navigation?.NodeType;

    /// <summary>Every trace in a match - <paramref name="trace"/> itself and every capture beneath it, in text order.</summary>
    public static IEnumerable<CaptureTrace> SelfAndDescendants(CaptureTrace trace) =>
        CaptureTraceWalker.GetSegments(trace)
            .Where(x => x.Child is not null)
            .SelectMany(x => SelfAndDescendants(x.Child))
            .Prepend(trace);
}
