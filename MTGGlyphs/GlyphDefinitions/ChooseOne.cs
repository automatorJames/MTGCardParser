namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The head of a modal spell or ability: "choose one —", "choose two —", "choose one or both —", "choose one or more —", "choose one that hasn't been chosen —", "choose one that hasn't been chosen this turn —", or with another player choosing, "an opponent chooses one —"; each mode follows on its own line (see <see cref="Mode"/>). Without the dash when a rule about the modes follows first: "choose four. you may choose the same mode more than once."</summary>
/// <exampledoc>Blue Elemental Blast</exampledoc>
/// <examplecapture>choose one —</examplecapture>
public class ChooseOne : Glyph
{
    public override Nib[] Nibs => [Prop(Chooser), Alt("choose", "chooses"), Prop(Count), Prop(Unchosen), Prop(Dash)];

    [Optional]
    public WhichPlayer? Chooser { get; set; }
    public ModeCount Count { get; set; }
    [Optional]
    public UnchosenMode? Unchosen { get; set; }
    [RegexPattern("—")]
    public bool Dash { get; set; }
}
