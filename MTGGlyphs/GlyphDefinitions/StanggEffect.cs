namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Stangg, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stangg</exampledoc>
[Dependent]
public class StanggEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create", Nib.This, "twin, a legendary 3/4 red and green human warrior creature token"];
}
