namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Outlaw Stitcher, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Outlaw Stitcher</exampledoc>
/// <examplecapture>create a 2/2 blue and black zombie rogue creature token, then put two +1/+1 counters on that token for each spell you've cast this turn other than the first</examplecapture>
[Dependent]
public class OutlawStitcherEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 2/2 blue and black zombie rogue creature token, then put two +1/+1 counters on that token for each spell you've cast this turn other than the first"];
}
