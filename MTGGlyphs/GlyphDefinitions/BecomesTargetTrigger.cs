namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger on a permanent becoming the target of a spell or ability: "whenever this creature becomes the target of a spell or ability an opponent controls, …" (ward's reminder text, heroic-like triggers).</summary>
/// <exampledoc>Hexbane Tortoise</exampledoc>
/// <examplecapture>whenever this creature becomes the target of a spell or ability an opponent controls, counter it unless that player pays {2}</examplecapture>
public class BecomesTargetTrigger : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Subject), "becomes the target of a spell or ability", Prop(Controller), ",", Prop(Effect)];

    public PermanentPhrase Subject { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
    public DynamicGlyph Effect { get; set; }
}
