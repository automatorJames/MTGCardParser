namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Players picked out by what they did or have: "each player who controls a white creature", "each player who searched their library this way", "each player who does", "each player who can't".</summary>
/// <exampledoc>Depopulate</exampledoc>
/// <examplecapture>each player who controls a multicolored creature</examplecapture>
[Dependent]
public class PlayersWho : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "who", Prop(Which)];

    public WhichPlayer Player { get; set; }
    public PlayerRelative Which { get; set; }
}
