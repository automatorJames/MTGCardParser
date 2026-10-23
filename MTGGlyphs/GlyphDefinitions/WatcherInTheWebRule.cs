namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Watcher in the Web, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Watcher in the Web</exampledoc>
[TokenizationOrder(-1)]
public class WatcherInTheWebRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature can block an additional seven creatures each combat"];
}
