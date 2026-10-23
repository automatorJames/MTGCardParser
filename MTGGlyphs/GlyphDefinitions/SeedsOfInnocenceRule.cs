namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Seeds of Innocence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Seeds of Innocence</exampledoc>
[TokenizationOrder(-1)]
public class SeedsOfInnocenceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the controller of each of those artifacts gains life equal to its mana value"];
}
