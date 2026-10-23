namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shanna, Purifying Blade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shanna, Purifying Blade</exampledoc>
[TokenizationOrder(-1)]
public class ShannaPurifyingBladeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["x can't be greater than the amount of life you gained this turn"];
}
