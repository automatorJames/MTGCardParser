namespace MTGRulesIR.Lowering;

/// <summary>
/// What one line's lowering knows beyond the glyph in hand: the document and line, the subject of the clause being
/// lowered (so a predicate like "draws three cards" knows who draws), the bindings given to selectors that introduce
/// referents, and the capture traces behind the hydrated objects - which is where a nested glyph's own text, and the
/// glyph an antecedent was captured inside, come from.
/// </summary>
public sealed class LoweringContext
{
    public IDocument Document { get; }
    public ProcessedLine Line { get; }

    /// <summary>The card's types ("Creature", "Instant", "Artifact,Creature"), which decide what frame an effect-shaped line is.</summary>
    public string Types { get; }

    /// <summary>The subject of the clause being lowered - "target player" in "target player draws three cards" - or null in an imperative.</summary>
    public Selector Subject { get; set; }

    /// <summary>How many back-references on the line the lowering could not bind to a lowered selector.</summary>
    public int UnboundReferences { get; private set; }

    readonly Dictionary<object, string> _bindings = new(ReferenceEqualityComparer.Instance);
    readonly Dictionary<object, CaptureTrace> _traces = new(ReferenceEqualityComparer.Instance);
    readonly Dictionary<object, object> _enclosingByValue = new(ReferenceEqualityComparer.Instance);
    readonly Dictionary<CaptureTrace, object> _enclosingByTrace = [];

    public LoweringContext(IDocument document, ProcessedLine line)
    {
        Document = document;
        Line = line;
        Types = (document as MTGGlyphs.Data.Card)?.Types ?? "";

        foreach (var glyph in line.Glyphs.OfType<Glyph>())
            Index(glyph.CaptureContext.RootCaptureTrace, null);
    }

    /// <summary>
    /// Records every capture of the line: the trace each hydrated object came from (first occurrence wins), and the
    /// nearest glyph enclosing each capture - looking through one-ofs, wrappers and dynamics, which hold their
    /// contents rather than own them.
    /// </summary>
    void Index(CaptureTrace trace, object enclosing)
    {
        var value = trace.ClrValue;

        if (value is not null)
        {
            _traces.TryAdd(value, trace);
            _enclosingByValue.TryAdd(value, enclosing);
        }

        _enclosingByTrace.TryAdd(trace, enclosing);

        var inner = value is Glyph glyph && !Lowerer.IsTransparent(glyph.GetType()) && glyph is not BackReference ? glyph : enclosing;

        foreach (var child in trace.EffectiveChildren.SelectMany(x => x))
            Index(child, inner);
    }

    /// <summary>The text <paramref name="value"/> was captured from, when it was captured on this line; else its name.</summary>
    public string TextOf(object value) =>
        value switch
        {
            null => null,
            _ when _traces.TryGetValue(value, out var trace) => trace.CaptureValue,
            CaptureUnit unit => unit.CaptureValue,
            Enum member => member.ToString(),
            _ => value.ToString(),
        };

    /// <summary>The binding of <paramref name="glyph"/>, a selector-producing glyph that introduces a referent - "#1", "#2", … in order of binding.</summary>
    public string Bind(object glyph)
    {
        if (!_bindings.TryGetValue(glyph, out var id))
            _bindings[glyph] = id = $"#{_bindings.Count + 1}";

        return id;
    }

    /// <summary>
    /// <paramref name="reference"/> as a selector: the card itself when it refers to the document, else a
    /// <see cref="Model.Reference"/> to the binding of the selector that introduced its antecedent - or to nothing,
    /// when that selector wasn't lowered or the grammar left the reference unresolved.
    /// </summary>
    public Selector Reference(BackReference reference)
    {
        var text = TextOf(reference);
        var antecedent = reference.Antecedent;

        if (antecedent is null)
        {
            UnboundReferences++;
            return new Reference(null, text);
        }

        if (antecedent.IsSelf)
            return new Self();

        var owner = OwnerOf(antecedent);

        if (owner is This or G.ThisCreature or G.ThisSpell or G.ShortName or G.ShortNameSubject or G.ShortNameObject or G.PossessedShortName)
            return new Self();

        if (owner is not null && _bindings.TryGetValue(owner, out var id))
            return new Reference(id, text, antecedent.Text);

        UnboundReferences++;
        return new Reference(null, text, antecedent.Text);
    }

    /// <summary>The glyph that introduced <paramref name="antecedent"/>: the capture itself when its type is the referent, else the glyph the referent property belongs to.</summary>
    object OwnerOf(ReferentCapture antecedent)
    {
        if (antecedent.Value is Glyph glyph && glyph.GetType().IsDefined(typeof(ReferentAttribute)))
            return glyph;

        if (antecedent.Value is not null && _enclosingByValue.TryGetValue(antecedent.Value, out var byValue))
            return byValue;

        if (antecedent.Trace is not null && _enclosingByTrace.TryGetValue(antecedent.Trace, out var byTrace))
            return byTrace;

        return null;
    }
}
