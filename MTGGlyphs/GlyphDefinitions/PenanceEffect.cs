namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Penance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Penance</exampledoc>
/// <examplecapture>the next time a black or red source of your choice would deal damage this turn, prevent that damage</examplecapture>
[Dependent]
public class PenanceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next time a black or red source of your choice would deal damage this turn, prevent that damage"];
}
