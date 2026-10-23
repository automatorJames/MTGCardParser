namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mad Dog, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mad Dog</exampledoc>
[Dependent]
public class MadDogEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this creature didn't attack or come under your control this turn, sacrifice it"];
}
