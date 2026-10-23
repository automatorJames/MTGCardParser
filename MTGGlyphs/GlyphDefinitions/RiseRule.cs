namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rise // Fall, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rise // Fall</exampledoc>
[TokenizationOrder(-1)]
public class RiseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target creature card from a graveyard and target creature on the battlefield to their owners' hands"];
}
