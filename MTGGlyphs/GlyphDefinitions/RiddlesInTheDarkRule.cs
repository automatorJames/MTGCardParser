namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Riddles in the Dark, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Riddles in the Dark</exampledoc>
[TokenizationOrder(-1)]
public class RiddlesInTheDarkRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["look at the top four cards of your library and separate them into a face-down pile and a face-up pile"];
}
