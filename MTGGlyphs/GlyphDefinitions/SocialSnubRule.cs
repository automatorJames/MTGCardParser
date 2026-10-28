namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Social Snub, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Social Snub</exampledoc>
[TokenizationOrder(-1)]
public class SocialSnubRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you cast this spell while you control a creature, you may copy this spell"];
}
