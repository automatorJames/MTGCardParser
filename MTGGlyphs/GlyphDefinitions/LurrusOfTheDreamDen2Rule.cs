namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lurrus of the Dream-Den, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lurrus of the Dream-Den</exampledoc>
[TokenizationOrder(-1)]
public class LurrusOfTheDreamDen2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["companion — each permanent card in your starting deck has mana value 2 or less"];
}
