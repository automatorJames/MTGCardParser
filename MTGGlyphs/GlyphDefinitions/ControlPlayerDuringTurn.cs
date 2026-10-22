namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Controlling another player's turn (Mindslaver): "control target player during that player's next turn", "control target opponent during their next combat phase".</summary>
/// <exampledoc>Mindslaver</exampledoc>
/// <examplecapture>control target player during that player's next turn</examplecapture>
[Dependent]
public class ControlPlayerDuringTurn : Glyph, IPredicate
{
    public override Nib[] Nibs => ["control", Prop(Player), "during", Pattern("(that player's|their)"), "next", Prop(Period)];

    public OneOf<TargetedPlayer, PlayerReference> Player { get; set; }
    public ControlledPeriod Period { get; set; }
}
