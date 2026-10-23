namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Umori, the Collector, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Umori, the Collector</exampledoc>
[TokenizationOrder(-1)]
public class UmoriTheCollectorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["companion — each nonland card in your starting deck shares a card type"];
}
