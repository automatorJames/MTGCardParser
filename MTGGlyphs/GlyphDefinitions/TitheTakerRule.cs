namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tithe Taker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tithe Taker</exampledoc>
/// <examplecapture>during your turn, spells your opponents cast cost {1} more to cast and abilities your opponents activate cost {1} more to activate unless they're mana abilities</examplecapture>
[TokenizationOrder(-1)]
public class TitheTakerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during your turn, spells your opponents cast cost {1} more to cast and abilities your opponents activate cost {1} more to activate unless they're mana abilities"];
}
