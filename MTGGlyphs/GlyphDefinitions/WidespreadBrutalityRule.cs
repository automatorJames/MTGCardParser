namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Widespread Brutality, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Widespread Brutality</exampledoc>
[TokenizationOrder(-1)]
public class WidespreadBrutalityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["amass zombies 2, then the army you amassed deals damage equal to its power to each non-army creature"];
}
