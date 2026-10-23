namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Obeka, Splitter of Seconds, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Obeka, Splitter of Seconds</exampledoc>
[Dependent]
public class ObekaSplitterOfSecondsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you get that many additional upkeep steps after this phase"];
}
