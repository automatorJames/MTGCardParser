namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Prismari Apprentice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Prismari Apprentice</exampledoc>
[TokenizationOrder(-1)]
public class PrismariApprenticeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that spell has mana value 5 or greater, put a +1/+1 counter on this creature"];
}
