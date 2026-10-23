namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Case of the Gateway Express, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Case of the Gateway Express</exampledoc>
[TokenizationOrder(-1)]
public class CaseOfTheGatewayExpressRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["to solve — three or more creatures attacked this turn"];
}
