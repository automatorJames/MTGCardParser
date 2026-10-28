namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Taii Wakeen, Perfect Shot, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Taii Wakeen, Perfect Shot</exampledoc>
[TokenizationOrder(-1)]
public class TaiiWakeenPerfectShotRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a source you control deals noncombat damage to a creature equal to that creature's toughness, draw a card"];
}
