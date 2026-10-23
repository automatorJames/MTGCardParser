namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Leafkin Avenger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Leafkin Avenger</exampledoc>
[Dependent]
public class LeafkinAvengerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add {g} for each creature with power 4 or greater you control"];
}
