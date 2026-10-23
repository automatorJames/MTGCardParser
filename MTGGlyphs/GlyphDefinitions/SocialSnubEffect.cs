namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Social Snub, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Social Snub</exampledoc>
[Dependent]
public class SocialSnubEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may copy this spell"];
}
