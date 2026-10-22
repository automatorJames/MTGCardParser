namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"each player's", "each opponent's", "each other player's": every player (or every player but you) in turn, a referent for a later "that player".</summary>
/// <exampledoc>Copper Tablet</exampledoc>
/// <examplecapture>each player's</examplecapture>
[Dependent]
public class EachPlayers : Glyph
{
    public override Nib[] Nibs => ["each", Prop(Other), Prop(Player), "'s"];

    [Optional]
    [RegexPattern("other")]
    public bool? Other { get; set; }
    [Referent]
    [Singular]
    public PlayerIdentity Player { get; set; }
}
