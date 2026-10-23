namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Skull Raid, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skull Raid</exampledoc>
[TokenizationOrder(-1)]
public class SkullRaidRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if fewer than two cards were discarded this way, you draw cards equal to the difference"];
}
