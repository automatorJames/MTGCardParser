namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Omenpath Journey, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Omenpath Journey</exampledoc>
[Dependent]
public class OmenpathJourney2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for up to five land cards that have different names, exile them, then shuffle"];
}
