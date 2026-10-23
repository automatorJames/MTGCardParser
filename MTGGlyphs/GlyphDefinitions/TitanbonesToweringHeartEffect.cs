namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Titanbones, Towering Heart, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Titanbones, Towering Heart</exampledoc>
[Dependent]
public class TitanbonesToweringHeartEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain 3 life"];
}
