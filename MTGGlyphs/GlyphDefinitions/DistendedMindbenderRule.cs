namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Distended Mindbender, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Distended Mindbender</exampledoc>
[TokenizationOrder(-1)]
public class DistendedMindbenderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you choose from it a nonland card with mana value 3 or less and a card with mana value 4 or greater"];
}
