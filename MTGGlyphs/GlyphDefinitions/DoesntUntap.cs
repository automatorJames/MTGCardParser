namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent that stays tapped through its controller's untap step, possibly only while a condition holds: "doesn't untap during your untap step", "doesn't untap during your untap step if it has a depletion counter on it", "doesn't untap during its controller's untap step for as long as {this} remains tapped".</summary>
/// <exampledoc>Paralyze</exampledoc>
/// <examplecapture>doesn't untap during its controller's untap step</examplecapture>
[Dependent]
public class DoesntUntap : Glyph, IPredicate
{
    public override Nib[] Nibs => [Alt("doesn't", "don't"), "untap during", Prop(Step), Prop(If)];

    public PlayersPhase Step { get; set; }
    [Optional]
    public OneOf<IfState, ForAsLongAs> If { get; set; }
}
