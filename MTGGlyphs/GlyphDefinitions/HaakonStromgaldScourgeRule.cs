namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Haakon, Stromgald Scourge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haakon, Stromgald Scourge</exampledoc>
[TokenizationOrder(-1)]
public class HaakonStromgaldScourgeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast this card from your graveyard, but not from anywhere else"];
}
