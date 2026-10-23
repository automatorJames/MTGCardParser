namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lich's Mirror, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lich's Mirror</exampledoc>
/// <examplecapture>if you would lose the game, instead shuffle your hand, your graveyard, and all permanents you own into your library, then draw seven cards and your life total becomes 20</examplecapture>
[TokenizationOrder(-1)]
public class LichsMirrorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would lose the game, instead shuffle your hand, your graveyard, and all permanents you own into your library, then draw seven cards and your life total becomes 20"];
}
