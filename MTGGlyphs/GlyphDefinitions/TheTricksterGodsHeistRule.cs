namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Trickster-God's Heist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Trickster-God's Heist</exampledoc>
[TokenizationOrder(-1)]
public class TheTricksterGodsHeistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ii — you may exchange control of two target nonbasic, noncreature permanents that share a card type"];
}
