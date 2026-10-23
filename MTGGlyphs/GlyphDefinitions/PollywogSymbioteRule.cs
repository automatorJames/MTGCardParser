namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pollywog Symbiote, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pollywog Symbiote</exampledoc>
[TokenizationOrder(-1)]
public class PollywogSymbioteRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature spell you cast costs {1} less to cast if it has mutate"];
}
