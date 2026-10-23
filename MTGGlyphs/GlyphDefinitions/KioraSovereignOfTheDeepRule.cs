namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kiora, Sovereign of the Deep, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kiora, Sovereign of the Deep</exampledoc>
[TokenizationOrder(-1)]
public class KioraSovereignOfTheDeepRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast a spell with mana value less than x from among them without paying its mana cost"];
}
