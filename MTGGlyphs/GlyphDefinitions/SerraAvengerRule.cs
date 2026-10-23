namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Serra Avenger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Serra Avenger</exampledoc>
/// <examplecapture>you can't cast {this} during your first, second, or third turns of the game</examplecapture>
[TokenizationOrder(-1)]
public class SerraAvengerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't cast", Nib.This, "during your first, second, or third turns of the game"];
}
