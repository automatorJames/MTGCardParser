namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"won't untap during your next untap step": a permanent staying tapped through its controller's next untap step (exert's reminder, said of "it").</summary>
/// <exampledoc>Battlefield Scavenger</exampledoc>
/// <examplecapture>won't untap during your next untap step</examplecapture>
[Dependent]
public class WontUntap : Glyph, IPredicate
{
    public override Nib[] Nibs => ["won't untap during", Prop(Step)];

    public NextPlayersPhase Step { get; set; }
}
