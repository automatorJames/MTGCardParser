namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of PuPu UFO, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>PuPu UFO</exampledoc>
[Dependent]
public class PuPuUFOEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, this creature's base power becomes equal to the number of towns you control"];
}
