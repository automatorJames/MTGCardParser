namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rise from the Wreck, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rise from the Wreck</exampledoc>
/// <examplecapture>return up to one target creature card, up to one target mount card, up to one target vehicle card, and up to one target creature card with no abilities from your graveyard to your hand</examplecapture>
[TokenizationOrder(-1)]
public class RiseFromTheWreckRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return up to one target creature card, up to one target mount card, up to one target vehicle card, and up to one target creature card with no abilities from your graveyard to your hand"];
}
