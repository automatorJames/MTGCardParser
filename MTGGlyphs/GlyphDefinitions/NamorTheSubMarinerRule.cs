namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Namor the Sub-Mariner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Namor the Sub-Mariner</exampledoc>
[TokenizationOrder(-1)]
public class NamorTheSubMarinerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast a noncreature spell with one or more blue mana symbols in its mana cost, create that many 1/1 blue merfolk creature tokens"];
}
