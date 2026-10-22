namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Turning a manifested or morph card face up for its mana cost, for a creature card: "turn it face up any time for its mana cost if it's a creature card", or in the third-person reminder, "if it's a creature card, it can be turned face up any time for its mana cost".</summary>
/// <exampledoc>Gift of Doom</exampledoc>
/// <examplecapture>turn it face up any time for its mana cost if it's a creature card</examplecapture>
public class TurnFaceUpForManaCost : Glyph
{
    public override Nib[] Nibs => [Alt("turn it face up any time for its mana cost if it's a creature card", "if it's a creature card, it can be turned face up any time for its mana cost")];
}
