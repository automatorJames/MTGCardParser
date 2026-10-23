namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Groom's Finery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Groom's Finery</exampledoc>
[TokenizationOrder(-1)]
public class GroomsFineryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gets an additional +0/+2 and has deathtouch as long as an equipment named bride's gown is attached to a creature you control"];
}
