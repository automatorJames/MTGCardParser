namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Selective Snare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Selective Snare</exampledoc>
[TokenizationOrder(-1)]
public class SelectiveSnareRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return x target creatures of the creature type of your choice to their owner's hand"];
}
