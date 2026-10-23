namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Toph, Hardheaded Teacher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Toph, Hardheaded Teacher</exampledoc>
[TokenizationOrder(-1)]
public class TophHardheadedTeacherRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that spell is a lesson, put an additional +1/+1 counter on that land"];
}
