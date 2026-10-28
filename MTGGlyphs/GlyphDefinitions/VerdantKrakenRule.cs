namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Verdant Kraken, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Verdant Kraken</exampledoc>
[TokenizationOrder(-1)]
public class VerdantKrakenRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it's affected by summoning sickness until your next turn"];
}
