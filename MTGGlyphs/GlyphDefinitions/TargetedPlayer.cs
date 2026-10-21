namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target player", "target opponent", "another target player": a targeted player, where only a player fits (as the controller in "creatures target player controls"). A referent, so a later "that player" refers to it.</summary>
/// <exampledoc>Dawnglare Invoker</exampledoc>
/// <examplecapture>target player</examplecapture>
[Dependent]
public class TargetedPlayer : Glyph
{
    public override Nib[] Nibs => [Prop(Another), "target", Prop(Player)];

    [RegexPattern("another")]
    public bool Another { get; set; }
    [Referent]
    public PlayerIdentity Player { get; set; }
}
