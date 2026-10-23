namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static ability of the enchanted permanent whose details aren't modelled yet: "enchanted land has indestructible and can't be enchanted by other auras", "enchanted land is a swamp".</summary>
/// <exampledoc>Evil Presence</exampledoc>
/// <examplecapture>enchanted land is a swamp</examplecapture>
public class EnchantedStatic : Glyph
{
    public override Nib[] Nibs => ["enchanted", Prop(Kind), Prop(Ability)];

    public CardType Kind { get; set; }
    public DynamicGlyph Ability { get; set; }
}
