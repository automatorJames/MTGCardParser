namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spikeshell Harrier, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spikeshell Harrier</exampledoc>
[TokenizationOrder(-1)]
public class SpikeshellHarrierRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that opponent's speed is greater than each other player's speed, reduce that opponent's speed by 1"];
}
