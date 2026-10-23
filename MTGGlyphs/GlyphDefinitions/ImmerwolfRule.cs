namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Immerwolf, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Immerwolf</exampledoc>
[TokenizationOrder(-1)]
public class ImmerwolfRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["non-human werewolves you control can't transform"];
}
