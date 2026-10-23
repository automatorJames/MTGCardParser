namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kianne, Dean of Substance // Imbraham, Dean of Theory, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kianne, Dean of Substance // Imbraham, Dean of Theory</exampledoc>
[TokenizationOrder(-1)]
public class KianneDeanOfSubstanceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on it for each different mana value among nonland cards you own in exile with study counters on them"];
}
