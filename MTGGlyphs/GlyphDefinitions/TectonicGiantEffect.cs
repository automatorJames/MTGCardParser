namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tectonic Giant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tectonic Giant</exampledoc>
[Dependent]
public class TectonicGiantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose one —"];
}
