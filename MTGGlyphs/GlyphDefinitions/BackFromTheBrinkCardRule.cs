namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Back from the Brink, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Back from the Brink</exampledoc>
[TokenizationOrder(-1)]
public class BackFromTheBrinkCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile a creature card from your graveyard and pay its mana cost: create a token that's a copy of that card"];
}
