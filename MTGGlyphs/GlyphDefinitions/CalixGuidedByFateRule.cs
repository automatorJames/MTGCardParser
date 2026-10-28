namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Calix, Guided by Fate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Calix, Guided by Fate</exampledoc>
[TokenizationOrder(-1)]
public class CalixGuidedByFateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever calix or an enchanted creature you control deals combat damage to a player, you may create a token that's a copy of a nonlegendary enchantment you control"];
}
