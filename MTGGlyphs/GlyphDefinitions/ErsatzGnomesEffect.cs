namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ersatz Gnomes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ersatz Gnomes</exampledoc>
[Dependent]
public class ErsatzGnomesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target spell becomes colorless"];
}
