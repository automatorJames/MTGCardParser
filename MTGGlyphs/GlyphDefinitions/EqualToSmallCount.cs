namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"equal to the number of charge counters on this artifact", "equal to the number of islands you control": exactly a small count, inside a with-qualifier.</summary>
/// <exampledoc>Aether Vial</exampledoc>
/// <examplecapture>equal to the number of charge counters on this artifact</examplecapture>
[Dependent]
public class EqualToSmallCount : Glyph
{
    public override Nib[] Nibs => ["equal to", Prop(Count)];

    public OneOf<ControlledKindCount, CountersOnObjectCount> Count { get; set; }
}
