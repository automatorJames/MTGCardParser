namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An aura's enchant ability: "enchant land", "enchant creature", optionally outside the battlefield ("enchant creature card in a graveyard").</summary>
/// <exampledoc>Consecrate Land</exampledoc>
/// <examplecapture>enchant land</examplecapture>
public class EnchantPermanent : Glyph
{
    public override Nib[] Nibs => ["enchant", Prop(CardOrCreatureType), Prop(CardOutsideBattlefield)];

    public OneOf<CardType?, CreatureType?> CardOrCreatureType { get; set; }

    [Optional]
    public CardOutsideBattlefield CardOutsideBattlefield { get; set; }
}