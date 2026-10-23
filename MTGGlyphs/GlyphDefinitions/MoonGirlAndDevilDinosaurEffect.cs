namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Moon Girl and Devil Dinosaur, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moon Girl and Devil Dinosaur</exampledoc>
[Dependent]
public class MoonGirlAndDevilDinosaurEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn,", Nib.This, "'s base power and toughness become 6/6 and they gain trample"];
}
