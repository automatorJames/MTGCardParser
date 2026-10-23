namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Liege of the Hollows, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liege of the Hollows</exampledoc>
[TokenizationOrder(-1)]
public class LiegeOfTheHollowsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then each player creates a number of 1/1 green squirrel creature tokens equal to the amount of mana they paid this way"];
}
