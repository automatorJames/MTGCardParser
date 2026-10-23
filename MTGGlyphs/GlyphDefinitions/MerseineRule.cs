namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Merseine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Merseine</exampledoc>
[TokenizationOrder(-1)]
public class MerseineRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only the controller of the enchanted creature may activate this ability"];
}
