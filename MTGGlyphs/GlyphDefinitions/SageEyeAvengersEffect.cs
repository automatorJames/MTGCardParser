namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sage-Eye Avengers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sage-Eye Avengers</exampledoc>
[Dependent]
public class SageEyeAvengersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may return target creature to its owner's hand if its power is less than this creature's power"];
}
