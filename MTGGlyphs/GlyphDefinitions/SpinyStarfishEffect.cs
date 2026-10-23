namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Spiny Starfish, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spiny Starfish</exampledoc>
/// <examplecapture>if this creature regenerated this turn, create a 0/1 blue starfish creature token for each time it regenerated this turn</examplecapture>
[Dependent]
public class SpinyStarfishEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this creature regenerated this turn, create a 0/1 blue starfish creature token for each time it regenerated this turn"];
}
