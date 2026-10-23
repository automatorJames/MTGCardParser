namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Desert Nomads, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Desert Nomads</exampledoc>
[TokenizationOrder(-1)]
public class DesertNomadsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["desertwalk"];
}
