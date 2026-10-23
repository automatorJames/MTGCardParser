namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Teferi, Who Slows the Sunset, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi, Who Slows the Sunset</exampledoc>
/// <examplecapture>you get an emblem with "untap all permanents you control during each opponent's untap step" and "you draw a card during each opponent's draw step</examplecapture>
[Dependent]
public class TeferiWhoSlowsTheSunsetEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you get an emblem with \"untap all permanents you control during each opponent's untap step\" and \"you draw a card during each opponent's draw step"];
}
