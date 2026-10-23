namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Scourglass, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scourglass</exampledoc>
[Dependent]
public class ScourglassEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all permanents except for artifacts and lands"];
}
