namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kastral, the Windcrested, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kastral, the Windcrested</exampledoc>
[TokenizationOrder(-1)]
public class KastralTheWindcrestedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• you may put a bird creature card from your hand or graveyard onto the battlefield with a finality counter on it"];
}
