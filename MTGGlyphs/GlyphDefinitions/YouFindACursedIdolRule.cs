namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of You Find a Cursed Idol, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>You Find a Cursed Idol</exampledoc>
[TokenizationOrder(-1)]
public class YouFindACursedIdolRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• steal its eyes — create a treasure token and venture into the dungeon"];
}
