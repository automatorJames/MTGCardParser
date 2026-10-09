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
