namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thousand Moons Smithy // Barracks of the Thousand, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thousand Moons Smithy // Barracks of the Thousand</exampledoc>
[TokenizationOrder(-1)]
public class ThousandMoonsSmithyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when thousand moons smithy enters, create a white gnome soldier artifact creature token with \"this token's power and toughness are each equal to the number of artifacts and/or creatures you control"];
}
