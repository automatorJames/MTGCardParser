namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it's an artifact creature", optionally "with power and toughness each equal to its mana value".</summary>
[Dependent]
public class TransformedType : Glyph
{
    public override Nib[] Nibs => ["it's", Alt("a", "an"), Prop(CardType), Prop(WithPowerToughnessEqual)];

    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> CardType { get; set; }

    [Optional]
    public WithPowerToughnessEqual WithPowerToughnessEqual { get; set; }
}
