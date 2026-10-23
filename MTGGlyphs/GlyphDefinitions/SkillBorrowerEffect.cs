namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Skill Borrower, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skill Borrower</exampledoc>
[Dependent]
public class SkillBorrowerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if any of the abilities use that card's name, use this creature's name instead"];
}
