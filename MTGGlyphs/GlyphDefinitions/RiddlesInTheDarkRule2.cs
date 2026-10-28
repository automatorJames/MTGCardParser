namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Riddles in the Dark, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Riddles in the Dark</exampledoc>
[TokenizationOrder(-1)]
public class RiddlesInTheDarkRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["an opponent chooses one of the piles"];
}
