namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Malcolm, Alluring Scoundrel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Malcolm, Alluring Scoundrel</exampledoc>
[TokenizationOrder(-1)]
public class MalcolmAlluringScoundrelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if there are four or more chorus counters on malcolm, you may cast the discarded card without paying its mana cost"];
}
