namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Turn the Earth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Turn the Earth</exampledoc>
[TokenizationOrder(-1)]
public class TurnTheEarthRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the owners of those cards shuffle them into their libraries"];
}
