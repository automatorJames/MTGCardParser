namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mind Carver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mind Carver</exampledoc>
[TokenizationOrder(-1)]
public class MindCarverRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gets +3/+1 instead as long as an opponent has eight or more cards in their graveyard"];
}
