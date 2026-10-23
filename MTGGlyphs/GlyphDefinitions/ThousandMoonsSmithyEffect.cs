namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Thousand Moons Smithy // Barracks of the Thousand, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thousand Moons Smithy // Barracks of the Thousand</exampledoc>
/// <examplecapture>create a white gnome soldier artifact creature token with "this token's power and toughness are each equal to the number of artifacts and/or creatures you control</examplecapture>
[Dependent]
public class ThousandMoonsSmithyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a white gnome soldier artifact creature token with \"this token's power and toughness are each equal to the number of artifacts and/or creatures you control"];
}
