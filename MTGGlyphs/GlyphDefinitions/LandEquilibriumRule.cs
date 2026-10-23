namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Land Equilibrium, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Land Equilibrium</exampledoc>
/// <examplecapture>if an opponent who controls at least as many lands as you do would put a land onto the battlefield, that player instead puts that land onto the battlefield then sacrifices a land of their choice</examplecapture>
[TokenizationOrder(-1)]
public class LandEquilibriumRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an opponent who controls at least as many lands as you do would put a land onto the battlefield, that player instead puts that land onto the battlefield then sacrifices a land of their choice"];
}
