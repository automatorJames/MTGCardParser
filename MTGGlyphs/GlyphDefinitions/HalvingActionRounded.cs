namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"mills half their library, rounded down", "discards half the cards in their hand, rounded up", "sacrifices half the creatures they control of their choice, rounded up": milling, discarding or sacrificing half of something, with its rounding. The comma is a literal, since a nested glyph can't open with one.</summary>
/// <exampledoc>Traumatize</exampledoc>
/// <examplecapture>mills half their library, rounded down</examplecapture>
[Dependent]
public class HalvingActionRounded : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Action), Prop(Factor), Prop(What), ",", Prop(Rounded)];

    public HalvingAction Action { get; set; }
    public ScaleFactor Factor { get; set; }
    public OneOf<PlayersZone, HalvedObjects> What { get; set; }
    public Rounding Rounded { get; set; }
}
