namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"each player's", "each opponent's": every player in turn, a referent for a later "that player".</summary>
/// <exampledoc>Copper Tablet</exampledoc>
/// <examplecapture>each player's</examplecapture>
[Dependent]
public class EachPlayers : Glyph
{
    public override Nib[] Nibs => ["each", Prop(Player), "'s"];

    [Referent]
    [Singular]
    public PlayerIdentity Player { get; set; }
}
