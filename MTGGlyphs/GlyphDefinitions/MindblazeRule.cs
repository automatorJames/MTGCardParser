namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mindblaze, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mindblaze</exampledoc>
[TokenizationOrder(-1)]
public class MindblazeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that library contains exactly the chosen number of cards with the chosen name,", Nib.This, "deals 8 damage to that player"];
}
