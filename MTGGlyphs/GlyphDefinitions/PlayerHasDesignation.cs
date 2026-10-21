namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A designation a player can have, as a condition: "you have the city's blessing". Used as a condition in "if …", "as long as …", and activation restrictions.</summary>
/// <exampledoc>Deadeye Brawler</exampledoc>
/// <examplecapture>you have the city's blessing</examplecapture>
[Dependent]
public class PlayerHasDesignation : Glyph
{
    public override Nib[] Nibs => [Prop(Player), Pattern("ha(s|ve)"), Prop(Designation)];

    public WhichPlayer Player { get; set; }
    public PlayerDesignation Designation { get; set; }
}
