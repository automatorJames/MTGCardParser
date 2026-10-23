namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Errant, Street Artist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Errant, Street Artist</exampledoc>
[Dependent]
public class ErrantStreetArtistEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy target spell you control that wasn't cast"];
}
