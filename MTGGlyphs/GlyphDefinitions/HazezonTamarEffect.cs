namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hazezon Tamar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hazezon Tamar</exampledoc>
/// <examplecapture>create x 1/1 sand warrior creature tokens that are red, green, and white at the beginning of your next upkeep, where x is the number of lands you control at that time</examplecapture>
[Dependent]
public class HazezonTamarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create x 1/1 sand warrior creature tokens that are red, green, and white at the beginning of your next upkeep, where x is the number of lands you control at that time"];
}
