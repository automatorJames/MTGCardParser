namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Overblaze, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Overblaze</exampledoc>
[TokenizationOrder(-1)]
public class OverblazeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each time target permanent would deal damage to a permanent or player this turn, it deals double that damage to that permanent or player instead"];
}
