namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Biblioplex Tomekeeper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Biblioplex Tomekeeper</exampledoc>
[TokenizationOrder(-1)]
public class BiblioplexTomekeeperRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• target creature becomes unprepared"];
}
