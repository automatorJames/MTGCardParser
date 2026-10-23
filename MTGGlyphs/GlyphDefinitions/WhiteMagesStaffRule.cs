namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of White Mage's Staff, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>White Mage's Staff</exampledoc>
[TokenizationOrder(-1)]
public class WhiteMagesStaffRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature gets +1/+1, has \"whenever this creature attacks, you gain 1 life,\" and is a cleric in addition to its other types"];
}
