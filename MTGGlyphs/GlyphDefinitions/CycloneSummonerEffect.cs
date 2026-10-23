namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cyclone Summoner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cyclone Summoner</exampledoc>
[Dependent]
public class CycloneSummonerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you cast it from your hand, return all permanents to their owners' hands except for giants, wizards, and lands"];
}
