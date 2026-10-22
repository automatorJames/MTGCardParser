namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A particular player's, as an owner: "target player's", "target opponent's", "that player's".</summary>
/// <exampledoc>Jester's Cap</exampledoc>
/// <examplecapture>target player's</examplecapture>
[Dependent]
public class PlayerPossessive : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "'s"];

    public OneOf<TargetedPlayer, ThatPlayer> Player { get; set; }
}
