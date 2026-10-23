namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Great Hall of Starnheim, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Great Hall of Starnheim</exampledoc>
[TokenizationOrder(-1)]
public class GreatHallOfStarnheimCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{w}{w}{b}, {t}, sacrifice this land and a creature you control: create a 4/4 white angel warrior creature token with flying and vigilance"];
}
