namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Aquamorph Entity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aquamorph Entity</exampledoc>
[TokenizationOrder(-1)]
public class AquamorphEntityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as this creature enters or is turned face up, it becomes your choice of 5/1 or 1/5"];
}
