namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dimensional Breach, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dimensional Breach</exampledoc>
/// <examplecapture>for as long as any of those cards remain exiled, at the beginning of each player's upkeep, that player returns one of the exiled cards they own to the battlefield</examplecapture>
[TokenizationOrder(-1)]
public class DimensionalBreachRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for as long as any of those cards remain exiled, at the beginning of each player's upkeep, that player returns one of the exiled cards they own to the battlefield"];
}
