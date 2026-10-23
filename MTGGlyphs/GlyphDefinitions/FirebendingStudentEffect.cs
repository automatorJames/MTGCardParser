namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Firebending Student, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Firebending Student</exampledoc>
[Dependent]
public class FirebendingStudentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature attacks, add x {r}"];
}
