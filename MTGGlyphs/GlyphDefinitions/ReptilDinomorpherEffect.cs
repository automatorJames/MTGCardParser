namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Reptil, Dinomorpher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reptil, Dinomorpher</exampledoc>
[Dependent]
public class ReptilDinomorpherEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, reptil becomes a dinosaur hero with base power and toughness 3/5 and gains reach and vigilance"];
}
