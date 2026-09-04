namespace MTGGlyphs.GlyphDefinitions;

public class EnchantPermanent : Glyph
{
    public override Nib[] Nibs => ["enchant", Prop(CardOrCreatureType), Prop(CardOutsideBattlefield)];

    public OneOf<CardType?, CreatureType?> CardOrCreatureType { get; set; }

    [Optional]
    public CardOutsideBattlefield CardOutsideBattlefield { get; set; }
}