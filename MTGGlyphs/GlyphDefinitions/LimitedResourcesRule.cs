namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Limited Resources, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Limited Resources</exampledoc>
[TokenizationOrder(-1)]
public class LimitedResourcesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players can't play lands as long as ten or more lands are on the battlefield"];
}
