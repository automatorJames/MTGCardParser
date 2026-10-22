namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"equal to the number of charge counters on this artifact", "equal to the number of islands you control", "equal to 1 plus the sacrificed creature's mana value": exactly a small count or simple sum, inside a with-qualifier.</summary>
/// <exampledoc>Aether Vial</exampledoc>
/// <examplecapture>equal to the number of charge counters on this artifact</examplecapture>
[Dependent]
public class EqualToSmallCount : Glyph
{
    public override Nib[] Nibs => ["equal to", Prop(Count)];

    public OneOf<ControlledKindCount, CountersOnObjectCount, SacrificedCharacteristicPlus> Count { get; set; }
}
