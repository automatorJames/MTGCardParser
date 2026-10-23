namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Illuna, Apex of Wishes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illuna, Apex of Wishes</exampledoc>
[TokenizationOrder(-1)]
public class IllunaApexOfWishesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put that card onto the battlefield or into your hand"];
}
