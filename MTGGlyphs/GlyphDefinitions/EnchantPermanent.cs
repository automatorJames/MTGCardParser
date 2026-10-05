namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The Enchant keyword: "enchant creature", "enchant creature card in a graveyard".</summary>
public class EnchantPermanent : Glyph
{
    public override Nib[] Nibs => ["enchant", Prop(CardOrCreatureType), Prop(CardOutsideBattlefield)];

    public CardOrCreatureType CardOrCreatureType { get; set; }

    [Optional]
    public CardOutsideBattlefield CardOutsideBattlefield { get; set; }
}
