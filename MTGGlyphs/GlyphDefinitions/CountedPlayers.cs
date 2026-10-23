namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Players counted one by one, as "for each" takes them: "opponent", "player", "other player", optionally picked out by "who" ("opponent who lost life this turn", "opponent who can't"). The player is a referent, so a later "that player" is each one in turn.</summary>
/// <exampledoc>Blatant Thievery</exampledoc>
/// <examplecapture>opponent</examplecapture>
[Dependent]
public class CountedPlayers : Glyph
{
    public override Nib[] Nibs => [Prop(Other), Prop(Player), Prop(Who)];

    [Optional]
    [RegexPattern("other")]
    public bool? Other { get; set; }
    [Referent]
    public PlayerIdentity Player { get; set; }
    [Optional]
    public WhoClause Who { get; set; }
}
