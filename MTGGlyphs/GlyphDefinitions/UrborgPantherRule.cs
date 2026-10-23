namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Urborg Panther, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Urborg Panther</exampledoc>
/// <examplecapture>sacrifice a creature named feral shadow, a creature named breathstealer, and this creature: search your library for a card named spirit of the night, put that card onto the battlefield, then shuffle</examplecapture>
[TokenizationOrder(-1)]
public class UrborgPantherRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice a creature named feral shadow, a creature named breathstealer, and this creature: search your library for a card named spirit of the night, put that card onto the battlefield, then shuffle"];
}
