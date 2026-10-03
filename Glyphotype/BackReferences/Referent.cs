namespace Glyphotype.BackReferences;

/// <summary>
/// Something a <see cref="BackReference"/> can refer back to: a captured value a Glyph introduced (see
/// <see cref="IntroducesAttribute"/>), where it was captured, and what a back-reference has to agree with to refer to it.
/// </summary>
/// <param name="Value">The captured value - a Glyph, or an enum or primitive captured by an introducing property.</param>
/// <param name="Trace">Where <paramref name="Value"/> was captured.</param>
/// <param name="Number">Its number.</param>
/// <param name="Kind">The kind of thing it is (e.g. "creature"), or null when unspecified.</param>
public sealed record Referent(object Value, CaptureTrace Trace, GrammaticalNumber Number, string Kind)
{
    /// <summary>The source text the referent was captured from.</summary>
    public string Text => Trace.CaptureValue;

    /// <summary>
    /// The referent <paramref name="value"/>, captured at <paramref name="trace"/>, introduces, with
    /// <paramref name="introduces"/>'s features (none, when it's null). A value that's itself a resolved back-reference
    /// stands for whatever it refers to, so a later pronoun joins the same chain - "destroy it. it can't be
    /// regenerated" - rather than stopping at the first pronoun.
    /// </summary>
    public static Referent Of(object value, CaptureTrace trace, IntroducesAttribute introduces) =>
        value is BackReference { Antecedent: { } antecedent }
            ? antecedent
            : new(value, trace, introduces?.Number ?? GrammaticalNumber.Unspecified, introduces?.Kind);

    public override string ToString() => Text;
}
