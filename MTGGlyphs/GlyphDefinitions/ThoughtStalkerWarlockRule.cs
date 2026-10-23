namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thought-Stalker Warlock, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thought-Stalker Warlock</exampledoc>
[TokenizationOrder(-1)]
public class ThoughtStalkerWarlockRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they lost life this turn, they reveal their hand, you choose a nonland card from it, and they discard that card"];
}
