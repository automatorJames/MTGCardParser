namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Frankenstein's Monster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Frankenstein's Monster</exampledoc>
[TokenizationOrder(-1)]
public class FrankensteinsMonsterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each creature card exiled this way, this creature enters with a +2/+0, +1/+1, or +0/+2 counter on it"];
}
