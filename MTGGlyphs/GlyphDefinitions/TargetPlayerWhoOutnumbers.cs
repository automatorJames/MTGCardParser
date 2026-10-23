namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A targeted player picked out by outnumbering the chooser: "target player who controls more creatures than they do and is their opponent" (the Oaths), "target opponent who has more life than you do as you activate this ability" (the Keepers).</summary>
/// <exampledoc>Oath of Druids</exampledoc>
/// <examplecapture>target player who controls more creatures than they do and is their opponent</examplecapture>
[Dependent]
public class TargetPlayerWhoOutnumbers : Glyph
{
    public override Nib[] Nibs => ["target", Prop(Player), "who", Prop(Measure), "than", Prop(Compared), Opt("do"), Prop(Proviso)];

    [Referent]
    [Singular]
    public PlayerIdentity Player { get; set; }
    public PlayerMeasure Measure { get; set; }
    public OneOf<WhichPlayer?, TheyPlayer, PlayerReference> Compared { get; set; }
    [Optional]
    public TargetPlayerProviso? Proviso { get; set; }
}
