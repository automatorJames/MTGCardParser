namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spikeshell Harrier, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spikeshell Harrier</exampledoc>
[TokenizationOrder(-1)]
public class SpikeshellHarrierRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this effect can't reduce their speed below 1"];
}
