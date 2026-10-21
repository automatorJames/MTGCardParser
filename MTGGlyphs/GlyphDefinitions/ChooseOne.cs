namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The head of a modal spell or ability: "choose one —", "choose two —", "choose one or both —", "choose one or more —"; each mode follows on its own line (see <see cref="Mode"/>). Without the dash when a rule about the modes follows first: "choose four. you may choose the same mode more than once."</summary>
/// <exampledoc>Blue Elemental Blast</exampledoc>
/// <examplecapture>choose one —</examplecapture>
public class ChooseOne : Glyph
{
    public override Nib[] Nibs => ["choose", Prop(Count), Prop(Dash)];

    public ModeCount Count { get; set; }
    [RegexPattern("—")]
    public bool Dash { get; set; }
}
