namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Illuminated Folio, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illuminated Folio</exampledoc>
[Dependent]
public class IlluminatedFolioEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card"];
}
