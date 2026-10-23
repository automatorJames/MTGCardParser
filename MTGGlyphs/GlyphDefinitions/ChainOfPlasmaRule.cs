namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chain of Plasma, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chain of Plasma</exampledoc>
[TokenizationOrder(-1)]
public class ChainOfPlasmaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then that player or that permanent's controller may discard a card"];
}
