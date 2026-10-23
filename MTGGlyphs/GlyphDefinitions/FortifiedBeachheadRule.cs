namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fortified Beachhead, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fortified Beachhead</exampledoc>
[TokenizationOrder(-1)]
public class FortifiedBeachheadRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this land enters tapped unless you revealed a soldier card this way or you control a soldier"];
}
