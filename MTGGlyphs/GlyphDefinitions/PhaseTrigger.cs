namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// A trigger at a point in a turn: "at the beginning of your upkeep, …", "at the beginning of each player's upkeep, …".
/// Each player is a referent, so the effect can say "that player" (see <see cref="ThatPlayer"/>).
/// </summary>
/// <exampledoc>Copper Tablet</exampledoc>
/// <examplecapture>at the beginning of each player's upkeep, {this} deals 1 damage to that player</examplecapture>
public class PhaseTrigger : Glyph
{
    public override Nib[] Nibs => [Prop(When), ",", Prop(Effect)];

    public AtOrUntilPlayerPhase When { get; set; }

    [AllowUnmatched]
    public DynamicGlyph Effect { get; set; }
}
