namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Plague Nurse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Plague Nurse</exampledoc>
[Dependent]
public class PlagueNurseEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["a player dealt combat damage by a creature with toxic also gets poison counters equal to that creature's total toxic value"];
}
