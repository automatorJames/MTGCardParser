namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Haakon, Stromgald Scourge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haakon, Stromgald Scourge</exampledoc>
[TokenizationOrder(-1)]
public class HaakonStromgaldScourgeRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as haakon is on the battlefield, you may cast knight spells from your graveyard"];
}
