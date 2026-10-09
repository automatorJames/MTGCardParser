namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static ability conditioned on the enchanted permanent: "as long as enchanted artifact isn't a creature, …".</summary>
/// <exampledoc>Animate Artifact</exampledoc>
/// <examplecapture>as long as enchanted artifact isn't a creature, it's an artifact creature with power and toughness each equal to its mana value</examplecapture>
public class AsLongAsEnchantedCardPredicateThenEffect : Glyph
{
    public override Nib[] Nibs => ["as long as enchanted", Prop(CardType), Prop(Assertion), Opt(Pattern("an?")), Prop(CardAspect), ",", Prop(Effect)];

    public CardType CardType{ get; set; }
    public Assertion Assertion { get; set; }
    public CardAspect CardAspect { get; set; }
    public DynamicGlyph Effect { get; set; }
}


//[Dependent]
/// <summary>"it's an artifact creature with power and toughness each equal to its mana value": a permanent made a creature, with its power and toughness set by its mana value.</summary>
/// <exampledoc>Animate Artifact</exampledoc>
/// <examplecapture>it's an artifact creature with power and toughness each equal to its mana value</examplecapture>
public class TestThing : Glyph
{
    public override Nib[] Nibs => ["it's", Pattern("an?"), Prop(CardType), "with power and toughness each equal to its mana value"];

    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> CardType { get; set; }

}