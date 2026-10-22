namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two conditions on the game state that must both hold: "an opponent controls a swamp and you control a plains" (the Legates).</summary>
/// <exampledoc>Cho-Arrim Legate</exampledoc>
/// <examplecapture>an opponent controls a swamp and you control a plains</examplecapture>
[Dependent]
public class BothPlayersControl : Glyph
{
    public override Nib[] Nibs => [Prop(First), "and", Prop(Second)];

    public PlayerControls First { get; set; }
    public PlayerControls Second { get; set; }
}
