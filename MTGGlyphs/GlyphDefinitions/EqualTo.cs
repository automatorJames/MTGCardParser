namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount given as "equal to" a count, a characteristic or another measure, possibly scaled: "equal to that creature's toughness", "equal to the number of swamps they control", "equal to twice the number of cards in your hand", "equal to half your life, rounded up".</summary>
/// <exampledoc>Karma</exampledoc>
/// <examplecapture>equal to the number of swamps they control</examplecapture>
[Dependent]
public class EqualTo : Glyph
{
    public override Nib[] Nibs => ["equal to", Prop(Measure)];

    public OneOf<ScaledAmount, ScaledAmountExact, Amount> Measure { get; set; }
}
