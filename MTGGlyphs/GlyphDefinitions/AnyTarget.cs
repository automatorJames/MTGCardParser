namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"any target", "any other target", and in a list of several, "another target", "a third target": a target that can be a player or a permanent (other than one already chosen, or than the source), optionally chosen by another player ("any target of an opponent's choice") or limited by what happened to it ("any target that was dealt damage this turn").</summary>
/// <exampledoc>Aladdin's Ring</exampledoc>
/// <examplecapture>any target</examplecapture>
[Dependent]
public class AnyTarget : Glyph
{
    public override Nib[] Nibs => [Prop(Which), "target", Prop(ChosenBy), Prop(Happened)];

    public AnyTargetChoice Which { get; set; }
    [Optional]
    public OfPlayersChoice ChosenBy { get; set; }
    [Optional]
    public ThatDidThisTurn Happened { get; set; }
}
