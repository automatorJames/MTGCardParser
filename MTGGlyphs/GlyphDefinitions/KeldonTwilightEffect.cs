namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Keldon Twilight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Keldon Twilight</exampledoc>
/// <examplecapture>if no creatures attacked this turn, that player sacrifices a creature of their choice that they controlled since the beginning of the turn</examplecapture>
[Dependent]
public class KeldonTwilightEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if no creatures attacked this turn, that player sacrifices a creature of their choice that they controlled since the beginning of the turn"];
}
