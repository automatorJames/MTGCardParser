namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spark Rupture, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spark Rupture</exampledoc>
/// <examplecapture>each planeswalker with one or more loyalty counters on it loses all abilities and is a creature with power and toughness each equal to the number of loyalty counters on it</examplecapture>
[TokenizationOrder(-1)]
public class SparkRuptureRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each planeswalker with one or more loyalty counters on it loses all abilities and is a creature with power and toughness each equal to the number of loyalty counters on it"];
}
