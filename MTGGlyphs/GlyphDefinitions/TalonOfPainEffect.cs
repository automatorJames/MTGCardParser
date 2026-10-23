namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Talon of Pain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Talon of Pain</exampledoc>
[Dependent]
public class TalonOfPainEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a charge counter on this artifact"];
}
