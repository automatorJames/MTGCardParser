namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gather the Pack, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gather the Pack</exampledoc>
/// <examplecapture>spell mastery — if there are two or more instant and/or sorcery cards in your graveyard, put up to two creature cards from among the revealed cards into your hand instead of one</examplecapture>
[TokenizationOrder(-1)]
public class GatherThePackRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spell mastery — if there are two or more instant and/or sorcery cards in your graveyard, put up to two creature cards from among the revealed cards into your hand instead of one"];
}
