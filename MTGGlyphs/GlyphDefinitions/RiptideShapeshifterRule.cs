namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Riptide Shapeshifter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Riptide Shapeshifter</exampledoc>
[TokenizationOrder(-1)]
public class RiptideShapeshifterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["reveal cards from the top of your library until you reveal a creature card of that type"];
}
