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
/// <item>A capture marked <see cref="ReferentAttribute"/> then becomes a referent for what follows.</item>
/// <item>Every <see cref="IDocument.ThisToken"/> - the document's reference to itself - is a singular referent where it
/// sits, whether a <see cref="This"/> glyph captured it or it's a literal in the text around a capture's children.</item>
/// </list>
/// Visiting a capture only after its contents is what keeps a phrase from being its own antecedent: in "creatures
/// blocking or blocked by it", "it" is resolved before the phrase around it has been introduced.
/// </para>
/// <para>
/// Referents live as long as their line. A line is the unit a document's text is split into, and the one place
/// all of a reference's context is guaranteed to be - for MTG, one ability.
/// </para>
/// </summary>
public static class BackReferenceResolver
{
    /// <summary>Resolves the back-references in <paramref name="units"/> - one line's tokens, in order - and returns each, in text order.</summary>
    public static List<BackReferenceResolution> Resolve(IEnumerable<CaptureUnit> units)
    {
        List<ReferentCapture> referents = [];
        List<BackReferenceResolution> resolutions = [];

        foreach (var glyph in units.OfType<Glyph>())
            Visit(glyph.CaptureContext.RootCaptureTrace, null, referents, resolutions);

        return resolutions;
    }

    static void Visit(CaptureTrace trace, CaptureTrace parent, List<ReferentCapture> referents, List<BackReferenceResolution> resolutions)
    {
        var position = trace.Index;

        foreach (var child in OrderedChildren(trace))
        {
            AddSelfReferences(trace, position, child.Index, referents);
            Visit(child, trace, referents, resolutions);
            position = Math.Max(position, child.End);
        }

        AddSelfReferences(trace, position, trace.End, referents);

        if (trace.ClrValue is BackReference backReference)
            resolutions.Add(Resolve(backReference, trace, parent, referents));

        if (trace.ClrValue is not null && GetReferentNumber(trace) is { } number)
            referents.Add(ReferentCapture.Of(trace.ClrValue, trace, number));
    }

    static BackReferenceResolution Resolve(BackReference backReference, CaptureTrace trace, CaptureTrace parent, List<ReferentCapture> referents)
    {
        if (DeclaringProperty(trace)?.GetCustomAttribute<RefersToAttribute>() is { } refersTo)
        {
            var target = parent is null ? null : OrderedChildren(parent).FirstOrDefault(x => DeclaringProperty(x)?.Name == refersTo.PropertyName);

            backReference.Antecedent = target?.ClrValue is { } value ? ReferentCapture.Of(value, target, GetReferentNumber(target) ?? GrammaticalNumber.Unspecified) : null;

            return new(backReference, trace, backReference.Antecedent is null ? BackReferenceResolutionKind.Unresolved : BackReferenceResolutionKind.Declared);
        }

        backReference.Antecedent = referents.LastOrDefault(backReference.AgreesWith);

        return new(backReference, trace, backReference.Antecedent is null ? BackReferenceResolutionKind.Unresolved : BackReferenceResolutionKind.Searched);
    }

    /// <summary>A self-reference for each <see cref="IDocument.ThisToken"/> in <paramref name="trace"/>'s own text from <paramref name="start"/> to <paramref name="end"/> - text none of its children captured.</summary>
    static void AddSelfReferences(CaptureTrace trace, int start, int end, List<ReferentCapture> referents)
    {
        var text = trace.CaptureValue ?? "";
        start = Math.Clamp(start - trace.Index, 0, text.Length);
        end = Math.Clamp(end - trace.Index, start, text.Length);

        for (var i = text.IndexOf(IDocument.ThisToken, start, end - start, StringComparison.Ordinal); i >= 0; i = text.IndexOf(IDocument.ThisToken, i + 1, end - i - 1, StringComparison.Ordinal))
            referents.Add(ReferentCapture.Self(trace));
    }

    /// <summary>
    /// The number of the referent <paramref name="trace"/>'s capture is, or null when it isn't one: its property is
    /// marked <see cref="ReferentAttribute"/> - the more specific declaration, whose number it takes - else its Glyph type.
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
