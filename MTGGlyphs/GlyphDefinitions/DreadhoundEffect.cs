namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dreadhound, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dreadhound</exampledoc>
[Dependent]
public class DreadhoundEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent loses 1 life"];
}
