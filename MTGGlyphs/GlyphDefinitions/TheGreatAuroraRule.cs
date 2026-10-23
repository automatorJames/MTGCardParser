namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Great Aurora, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Great Aurora</exampledoc>
[TokenizationOrder(-1)]
public class TheGreatAuroraRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player shuffles all cards from their hand and all permanents they own into their library, then draws that many cards"];
}
