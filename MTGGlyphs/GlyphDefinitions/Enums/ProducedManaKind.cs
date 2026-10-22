namespace MTGGlyphs.GlyphDefinitions;

public enum ProducedManaKind
{
    [RegexPattern("colorless mana")]
    Colorless,

    [RegexPattern("(one )?mana of a color of your choice")]
    ChosenColor
}
