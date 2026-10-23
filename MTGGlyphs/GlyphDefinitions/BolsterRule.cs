namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Bolster's reminder: choosing the creature with the least toughness among a group, then putting counters on it. The group and the counters are slots.</summary>
/// <exampledoc>Aven Tactician</exampledoc>
/// <examplecapture>choose a creature with the least toughness among creatures you control and put a +1/+1 counter on it</examplecapture>
public class BolsterRule : Glyph
{
    public override Nib[] Nibs => ["choose a creature with the least toughness among", Prop(Among), "and", Prop(PlaceCounters)];

    public Permanents Among { get; set; }
    public PlaceCounters PlaceCounters { get; set; }
}
