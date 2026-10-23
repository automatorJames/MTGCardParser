namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wonder Man, Hollywood Hero, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wonder Man, Hollywood Hero</exampledoc>
[TokenizationOrder(-1)]
public class WonderManHollywoodHero2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once? reduce the cost by his mana cost if he entered this turn"];
}
