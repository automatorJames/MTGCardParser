namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The permanent an Aura is attached to: "enchanted creature", "enchanted land".</summary>
[Dependent]
public class EnchantedPermanent : Glyph
{
    public override Nib[] Nibs => ["enchanted", Prop(CardOrCreatureType)];

    public CardOrCreatureType CardOrCreatureType { get; set; }
}
