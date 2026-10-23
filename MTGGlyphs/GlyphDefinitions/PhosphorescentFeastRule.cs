namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Phosphorescent Feast, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phosphorescent Feast</exampledoc>
[TokenizationOrder(-1)]
public class PhosphorescentFeastRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain 2 life for each green mana symbol in those cards' mana costs"];
}
