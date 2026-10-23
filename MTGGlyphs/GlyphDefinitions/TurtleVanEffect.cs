namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Turtle Van, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Turtle Van</exampledoc>
[Dependent]
public class TurtleVanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on target creature that crewed it this turn"];
}
