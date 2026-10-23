namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Imprison, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Imprison</exampledoc>
[Dependent]
public class Imprison2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may pay {1}"];
}
