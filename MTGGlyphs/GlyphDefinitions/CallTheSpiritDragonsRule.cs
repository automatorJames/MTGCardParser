namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Call the Spirit Dragons, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Call the Spirit Dragons</exampledoc>
[TokenizationOrder(-1)]
public class CallTheSpiritDragonsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you put +1/+1 counters on five dragons this way, you win the game"];
}
