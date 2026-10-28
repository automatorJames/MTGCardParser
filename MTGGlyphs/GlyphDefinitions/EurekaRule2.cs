namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Eureka, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eureka</exampledoc>
[TokenizationOrder(-1)]
public class EurekaRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["repeat this process until no one puts a card onto the battlefield"];
}
