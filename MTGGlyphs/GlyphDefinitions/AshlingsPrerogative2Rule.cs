namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ashling's Prerogative, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ashling's Prerogative</exampledoc>
[TokenizationOrder(-1)]
public class AshlingsPrerogative2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature without mana value of the chosen quality enters tapped"];
}
