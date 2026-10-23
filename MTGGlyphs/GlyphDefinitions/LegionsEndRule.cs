namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Legion's End, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Legion's End</exampledoc>
[TokenizationOrder(-1)]
public class LegionsEndRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then that player reveals their hand and exiles all cards with that name from their hand and graveyard"];
}
