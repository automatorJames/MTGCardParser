namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fang-Druid Summoner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fang-Druid Summoner</exampledoc>
/// <examplecapture>you may search your library and/or graveyard for a creature card with no abilities, reveal it, and put it into your hand</examplecapture>
[Dependent]
public class FangDruidSummonerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may search your library and/or graveyard for a creature card with no abilities, reveal it, and put it into your hand"];
}
