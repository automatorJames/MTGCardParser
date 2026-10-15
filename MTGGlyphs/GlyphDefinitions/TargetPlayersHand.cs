namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player's hand named by a target: "target opponent's hand". Lets later text refer back to that player ("that player").</summary>
/// <exampledoc>Abandon Hope</exampledoc>
/// <examplecapture>target opponent's hand</examplecapture>
[Dependent]
public class TargetPlayersHand : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "'s hand"];

    public SpecificTarget Player { get; set; }
}
