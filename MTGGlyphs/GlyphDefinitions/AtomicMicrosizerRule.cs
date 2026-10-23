namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Atomic Microsizer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Atomic Microsizer</exampledoc>
[TokenizationOrder(-1)]
public class AtomicMicrosizerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature can't be blocked this turn and has base power and toughness 1/1 until end of turn"];
}
