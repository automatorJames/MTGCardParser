namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pontiff of Blight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pontiff of Blight</exampledoc>
[Dependent]
public class PontiffOfBlightEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature has multiple instances of extort, each triggers separately"];
}
