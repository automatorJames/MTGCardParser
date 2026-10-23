namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Inspire Awe, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Inspire Awe</exampledoc>
[TokenizationOrder(-1)]
public class InspireAweRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all combat damage that would be dealt this turn except combat damage that would be dealt by enchanted creatures and enchantment creatures"];
}
