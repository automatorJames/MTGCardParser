namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sphinx's Approach, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sphinx's Approach</exampledoc>
[TokenizationOrder(-1)]
public class SphinxsApproachRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then you may exile this spell and four cards named", Nib.This, "from your graveyard"];
}
