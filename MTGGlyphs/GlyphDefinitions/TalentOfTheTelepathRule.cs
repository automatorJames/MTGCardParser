namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Talent of the Telepath, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Talent of the Telepath</exampledoc>
/// <examplecapture>spell mastery — if there are two or more instant and/or sorcery cards in your graveyard, you may cast up to two instant and/or sorcery spells from among the revealed cards instead of one</examplecapture>
[TokenizationOrder(-1)]
public class TalentOfTheTelepathRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spell mastery — if there are two or more instant and/or sorcery cards in your graveyard, you may cast up to two instant and/or sorcery spells from among the revealed cards instead of one"];
}
