namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Loot, the Anomaly, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Loot, the Anomaly</exampledoc>
[TokenizationOrder(-1)]
public class LootTheAnomalyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if loot's power is negative, he assigns combat damage as though his power were positive"];
}
