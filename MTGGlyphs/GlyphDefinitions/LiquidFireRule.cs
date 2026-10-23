namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Liquid Fire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liquid Fire</exampledoc>
[TokenizationOrder(-1)]
public class LiquidFireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "deals x damage to target creature and 5 minus x damage to that creature's controller, where x is the chosen number"];
}
