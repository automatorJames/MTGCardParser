namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Beamsplitter Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Beamsplitter Mage</exampledoc>
[TokenizationOrder(-1)]
public class BeamsplitterMageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the copy targets the chosen creature"];
}
