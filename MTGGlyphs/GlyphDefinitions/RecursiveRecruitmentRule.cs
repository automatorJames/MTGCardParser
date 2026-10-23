namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Recursive Recruitment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Recursive Recruitment</exampledoc>
[TokenizationOrder(-1)]
public class RecursiveRecruitmentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was cast from a graveyard, put a +1/+1 counter on each of them for every three cards in your graveyard"];
}
