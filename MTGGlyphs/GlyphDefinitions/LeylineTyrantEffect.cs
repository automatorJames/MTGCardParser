namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Leyline Tyrant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Leyline Tyrant</exampledoc>
[Dependent]
public class LeylineTyrantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may pay any amount of {r}"];
}
