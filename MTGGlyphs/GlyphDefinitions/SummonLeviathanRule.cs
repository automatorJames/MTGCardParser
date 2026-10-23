namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Summon: Leviathan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Summon: Leviathan</exampledoc>
[TokenizationOrder(-1)]
public class SummonLeviathanRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["i — return each creature that isn't a kraken, leviathan, merfolk, octopus, or serpent to its owner's hand"];
}
