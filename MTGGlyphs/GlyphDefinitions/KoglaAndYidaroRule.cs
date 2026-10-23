namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kogla and Yidaro, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kogla and Yidaro</exampledoc>
[TokenizationOrder(-1)]
public class KoglaAndYidaroRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle this card into your library from your graveyard, then draw a card"];
}
