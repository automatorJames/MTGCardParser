namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Birgi, God of Storytelling // Harnfel, Horn of Bounty, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Birgi, God of Storytelling // Harnfel, Horn of Bounty</exampledoc>
[TokenizationOrder(-1)]
public class BirgiGodOfStorytellingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures you control can boast twice during each of your turns rather than once"];
}
