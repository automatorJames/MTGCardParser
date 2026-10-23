namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Spider-Man 2099, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spider-Man 2099</exampledoc>
/// <examplecapture>if you've played a land or cast a spell this turn from anywhere other than your hand, {this} deals damage equal to his power to any target</examplecapture>
[Dependent]
public class SpiderMan2099Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you've played a land or cast a spell this turn from anywhere other than your hand,", Nib.This, "deals damage equal to his power to any target"];
}
