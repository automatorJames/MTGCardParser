namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hum of the Radix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hum of the Radix</exampledoc>
[TokenizationOrder(-1)]
public class HumOfTheRadixRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each artifact spell costs {1} more to cast for each artifact its controller controls"];
}
