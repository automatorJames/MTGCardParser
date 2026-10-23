namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Riku of Many Paths, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Riku of Many Paths</exampledoc>
[Dependent]
public class RikuOfManyPathsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose up to x, where x is the number of times you chose a mode for that spell —"];
}
