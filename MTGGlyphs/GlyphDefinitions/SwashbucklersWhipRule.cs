namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Swashbuckler's Whip, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swashbuckler's Whip</exampledoc>
[TokenizationOrder(-1)]
public class SwashbucklersWhipRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipped creature has reach, \"{2}, {t}: tap target artifact or creature,\" and \"{8}, {t}: discover 10"];
}
