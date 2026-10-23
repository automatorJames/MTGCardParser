namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shared Fate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shared Fate</exampledoc>
[TokenizationOrder(-1)]
public class SharedFateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player may look at cards they exiled with this enchantment, and they may play lands and cast spells from among those cards"];
}
