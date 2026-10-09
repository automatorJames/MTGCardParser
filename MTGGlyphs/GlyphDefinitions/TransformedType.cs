namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "it's an enchantment", "it's an artifact creature with power and toughness each equal to its mana value": a
/// permanent being, or becoming, a type.
/// </summary>
/// <exampledoc>Animate Artifact</exampledoc>
/// <examplecapture>it's an artifact creature with power and toughness each equal to its mana value</examplecapture>
[Dependent]
public class TransformedType : Glyph
{
    public override Nib[] Nibs => ["it's", Pattern("an?"), Prop(CardType), Prop(PowerToughness)];

    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> CardType { get; set; }

    [Optional]
    public WithPowerToughnessEqual PowerToughness { get; set; }
}
