namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Yorion, Sky Nomad, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yorion, Sky Nomad</exampledoc>
[TokenizationOrder(-1)]
public class YorionSkyNomadRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["companion — your starting deck contains at least twenty cards more than the minimum deck size"];
}
