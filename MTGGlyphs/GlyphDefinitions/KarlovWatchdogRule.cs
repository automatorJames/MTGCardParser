namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Karlov Watchdog, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Karlov Watchdog</exampledoc>
[TokenizationOrder(-1)]
public class KarlovWatchdogRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["permanents your opponents control can't be turned face up during your turn"];
}
