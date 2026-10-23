namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Monument to Perfection, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Monument to Perfection</exampledoc>
[Dependent]
public class MonumentToPerfectionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for a basic, sphere, or locus land card, reveal it, put it into your hand, then shuffle"];
}
