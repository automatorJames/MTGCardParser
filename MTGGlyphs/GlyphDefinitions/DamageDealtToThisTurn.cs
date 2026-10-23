namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount of damage having been dealt to a permanent this turn, as a condition: "4 or more damage was dealt to it this turn" (the zuberas).</summary>
/// <exampledoc>Rushing-Tide Zubera</exampledoc>
/// <examplecapture>4 or more damage was dealt to it this turn</examplecapture>
[Dependent]
public class DamageDealtToThisTurn : Glyph
{
    public override Nib[] Nibs => [Prop(Amount), "was dealt to", Prop(Recipient), "this turn"];

    public DamageThreshold Amount { get; set; }
    public OneOf<It, ThisCreature> Recipient { get; set; }
}
