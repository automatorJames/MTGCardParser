namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Curse of Surveillance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Curse of Surveillance</exampledoc>
/// <examplecapture>any number of target players other than that player each draw cards equal to the number of curses attached to that player</examplecapture>
[Dependent]
public class CurseOfSurveillanceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any number of target players other than that player each draw cards equal to the number of curses attached to that player"];
}
