namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tariff, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tariff</exampledoc>
[TokenizationOrder(-1)]
public class Tariff2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if two or more creatures a player controls are tied for greatest, that player chooses one"];
}
