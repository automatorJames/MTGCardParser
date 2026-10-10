namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the chosen player's": the player chosen as the permanent entered, a referent for a later "that player".</summary>
/// <exampledoc>Black Vise</exampledoc>
/// <examplecapture>the chosen player's</examplecapture>
[Dependent]
public class ChosenPlayers : Glyph
{
    public override Nib[] Nibs => ["the chosen", Prop(Player), "'s"];

    [Referent]
    [Singular]
    public PlayerIdentity Player { get; set; }
}
