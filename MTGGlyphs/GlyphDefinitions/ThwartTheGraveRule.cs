namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thwart the Grave, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thwart the Grave</exampledoc>
[TokenizationOrder(-1)]
public class ThwartTheGraveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target creature card and up to one target cleric, rogue, warrior, or wizard creature card from your graveyard to the battlefield"];
}
