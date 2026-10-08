namespace Glyphotype.BackReferences;

/// <summary>
/// Resolves every <see cref="BackReference"/> on a tokenized line to its <see cref="BackReference.Antecedent"/> - the pass after
/// tokenization that gives "it" and "they" their meaning, the way a compiler resolves names after parsing.
/// <para>
/// The line's captures are walked in text order, Glyph after Glyph and clause after clause, as one stream - so
/// whether two sentences were matched by one Glyph or two never changes what their pronouns resolve to. Each
/// capture is visited after everything inside it:
/// <list type="bullet">
/// <item>A back-reference is bound to the most recent referent before it that agrees with it (see
/// <see cref="BackReference.AgreesWith"/>) - or, if its property declares <see cref="RefersToAttribute"/>, to that
/// property's capture outright. With nothing agreeing, it's left unresolved: an unresolved back-reference is a gap the
/// grammar can see and close, while a guessed one would hide it.</item>
/// <item>A capture marked <see cref="ReferentAttribute"/> then becomes a referent for what follows - every
/// <see cref="This"/> among them, the document's reference to itself (see <see cref="Nib.This"/>).</item>
/// </list>
/// Visiting a capture only after its contents is what keeps a phrase from being its own antecedent: in "creatures
/// blocking or blocked by it", "it" is resolved before the phrase around it has been introduced.
/// </para>
/// <para>
/// Referents live as long as their line. A line is the unit a document's text is split into, and the one place
/// all of a reference's context is guaranteed to be - for MTG, one ability.
/// </para>
/// <para>
/// Resolution is part of tokenizing a line: <see cref="GlyphGrammar.Tokenize"/> runs it before returning, so every
/// back-reference it hands back is already resolved. <see cref="Collect"/> reports how each one was.
/// </para>
/// </summary>
public static class BackReferenceResolver
{
    /// <summary>Resolves the back-references in <paramref name="units"/> - one line's tokens, in order (see <see cref="GlyphGrammar.Tokenize"/>).</summary>
    internal static void Resolve(IEnumerable<CaptureUnit> units)
    {
        List<ReferentCapture> referents = [];

        foreach (var glyph in units.OfType<Glyph>())
            Visit(glyph.CaptureContext.RootCaptureTrace, null, referents);
    }

    /// <summary>Each back-reference in <paramref name="units"/> - one line's tokens, already resolved - and how it was resolved, in text order.</summary>
    public static List<BackReferenceResolution> Collect(IEnumerable<CaptureUnit> units)
    {
        List<BackReferenceResolution> resolutions = [];

        void Walk(CaptureTrace trace)
        {
            foreach (var child in OrderedChildren(trace))
                Walk(child);

            if (trace.ClrValue is BackReference backReference)
                resolutions.Add(new(backReference, trace));
        }

        foreach (var glyph in units.OfType<Glyph>())
            Walk(glyph.CaptureContext.RootCaptureTrace);

        return resolutions;
    }

    /// <summary>Whether the back-reference captured at <paramref name="trace"/> is bound by its glyph (see <see cref="RefersToAttribute"/>) rather than searched for.</summary>
    internal static bool IsDeclared(CaptureTrace trace) =>
        DeclaringProperty(trace)?.IsDefined(typeof(RefersToAttribute)) == true;

    static void Visit(CaptureTrace trace, CaptureTrace parent, List<ReferentCapture> referents)
    {
        foreach (var child in OrderedChildren(trace))
            Visit(child, trace, referents);

        if (trace.ClrValue is BackReference backReference)
            backReference.Antecedent = IsDeclared(trace) ? FindDeclared(trace, parent) : referents.LastOrDefault(backReference.AgreesWith);

        if (trace.ClrValue is not null && GetReferentNumber(trace) is { } number)
            referents.Add(ReferentCapture.Of(trace.ClrValue, trace, number));
    }

    /// <summary>What the back-reference captured at <paramref name="trace"/> is bound to by its <see cref="RefersToAttribute"/>: the named property's capture, among its siblings.</summary>
    static ReferentCapture FindDeclared(CaptureTrace trace, CaptureTrace parent)
    {
        var propertyName = DeclaringProperty(trace).GetCustomAttribute<RefersToAttribute>().PropertyName;
        var target = parent is null ? null : OrderedChildren(parent).FirstOrDefault(x => DeclaringProperty(x)?.Name == propertyName);

        return target?.ClrValue is { } value ? ReferentCapture.Of(value, target, GetReferentNumber(target) ?? GrammaticalNumber.Unspecified) : null;
    }

    /// <summary>
    /// The number of the referent <paramref name="trace"/>'s capture is, or null when it isn't one: declared beside the
    /// <see cref="ReferentAttribute"/> that makes it one - its property's, the more specific declaration, else its Glyph type's.
    /// </summary>
    static GrammaticalNumber? GetReferentNumber(CaptureTrace trace) =>
        DeclaringProperty(trace) is { } property && property.IsDefined(typeof(ReferentAttribute)) ? GrammaticalNumberAttribute.Of(property)
        : trace.ClrValue?.GetType() is { } type && type.IsDefined(typeof(ReferentAttribute)) ? GrammaticalNumberAttribute.Of(type)
        : null;

    /// <summary>The property <paramref name="trace"/> was captured for - null for a line's root captures, which no property holds.</summary>
    static PropertyInfo DeclaringProperty(CaptureTrace trace) =>
        trace.SourceNode?.Navigation?.Prop;

    /// <summary>Every occurrence of every child of <paramref name="trace"/>, in text order - the same walk the captures are displayed by (see <see cref="CaptureTraceWalker"/>).</summary>
    static IEnumerable<CaptureTrace> OrderedChildren(CaptureTrace trace) =>
        trace.EffectiveChildren.SelectMany(x => x).OrderBy(x => x.Index);
}
