namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lorehold, the Historian, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lorehold, the Historian</exampledoc>
[Dependent]
public class LoreholdTheHistorianEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast a card for its miracle cost when you draw it if it's the first card you drew this turn"];
}
