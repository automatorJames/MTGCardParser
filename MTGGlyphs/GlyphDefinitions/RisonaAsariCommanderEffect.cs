namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Risona, Asari Commander, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Risona, Asari Commander</exampledoc>
[Dependent]
public class RisonaAsariCommanderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["remove an indestructible counter from risona"];
}
