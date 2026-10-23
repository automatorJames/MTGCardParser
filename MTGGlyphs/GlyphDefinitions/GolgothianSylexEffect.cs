namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Golgothian Sylex, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Golgothian Sylex</exampledoc>
[Dependent]
public class GolgothianSylexEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each nontoken permanent with a name originally printed in the antiquities expansion is sacrificed by its controller"];
}
