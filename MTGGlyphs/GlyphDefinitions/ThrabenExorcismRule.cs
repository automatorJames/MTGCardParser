namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thraben Exorcism, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thraben Exorcism</exampledoc>
[TokenizationOrder(-1)]
public class ThrabenExorcismRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile target spirit, creature with disturb, or enchantment"];
}
