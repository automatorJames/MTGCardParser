namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Alpine Moon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alpine Moon</exampledoc>
[TokenizationOrder(-1)]
public class AlpineMoonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["lands your opponents control with the chosen name lose all land types and abilities, and they gain \"{t}: add one mana of any color"];
}
