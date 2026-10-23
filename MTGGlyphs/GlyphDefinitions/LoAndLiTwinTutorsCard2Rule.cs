namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lo and Li, Twin Tutors, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lo and Li, Twin Tutors</exampledoc>
[TokenizationOrder(-1)]
public class LoAndLiTwinTutorsCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["noble creatures you control and lesson spells you control have lifelink"];
}
