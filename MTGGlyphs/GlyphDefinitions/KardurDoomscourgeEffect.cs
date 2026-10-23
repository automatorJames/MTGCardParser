namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kardur, Doomscourge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kardur, Doomscourge</exampledoc>
/// <examplecapture>until your next turn, creatures your opponents control attack each combat if able and attack a player other than you if able</examplecapture>
[Dependent]
public class KardurDoomscourgeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until your next turn, creatures your opponents control attack each combat if able and attack a player other than you if able"];
}
