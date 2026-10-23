namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sarkhan the Mad, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sarkhan the Mad</exampledoc>
[TokenizationOrder(-1)]
public class SarkhanTheMadRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sarkhan deals damage to himself equal to that card's mana value"];
}
