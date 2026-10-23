namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Psychotic Episode, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychotic Episode</exampledoc>
[TokenizationOrder(-1)]
public class PsychoticEpisodeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player reveals their hand and the top card of their library"];
}
