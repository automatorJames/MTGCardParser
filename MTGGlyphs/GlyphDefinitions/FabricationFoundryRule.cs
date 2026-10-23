namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fabrication Foundry, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fabrication Foundry</exampledoc>
/// <examplecapture>{2}{w}, {t}, exile one or more other artifacts you control with total mana value x: return target artifact card with mana value x or less from your graveyard to the battlefield</examplecapture>
[TokenizationOrder(-1)]
public class FabricationFoundryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{2}{w}, {t}, exile one or more other artifacts you control with total mana value x: return target artifact card with mana value x or less from your graveyard to the battlefield"];
}
