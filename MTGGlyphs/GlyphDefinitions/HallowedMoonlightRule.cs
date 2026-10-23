namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hallowed Moonlight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hallowed Moonlight</exampledoc>
[TokenizationOrder(-1)]
public class HallowedMoonlightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, if a creature would enter and it wasn't cast, exile it instead"];
}
