namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Great Hall of Starnheim, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Great Hall of Starnheim</exampledoc>
[Dependent]
public class GreatHallOfStarnheimEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 4/4 white angel warrior creature token with flying and vigilance"];
}
