namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Academic Probation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Academic Probation</exampledoc>
[TokenizationOrder(-1)]
public class AcademicProbationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["opponents can't cast spells with the chosen name until your next turn"];
}
