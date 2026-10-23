namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hide in Plain Sight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hide in Plain Sight</exampledoc>
/// <examplecapture>look at the top five cards of your library, cloak two of them, and put the rest on the bottom of your library in a random order</examplecapture>
[TokenizationOrder(-1)]
public class HideInPlainSightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["look at the top five cards of your library, cloak two of them, and put the rest on the bottom of your library in a random order"];
}
