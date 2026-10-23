namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sphinx Ambassador, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sphinx Ambassador</exampledoc>
[TokenizationOrder(-1)]
public class SphinxAmbassadorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you searched for a creature card that doesn't have that name, you may put it onto the battlefield under your control"];
}
