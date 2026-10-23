namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Infinite Authority, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Infinite Authority</exampledoc>
[Dependent]
public class InfiniteAuthorityEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that creature was destroyed this way, put a +1/+1 counter on the first creature"];
}
