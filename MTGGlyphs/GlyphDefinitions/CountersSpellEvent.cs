namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger on countering: "a spell or ability you control counters a spell".</summary>
/// <exampledoc>Lullmage Mentor</exampledoc>
/// <examplecapture>a spell or ability you control counters a spell</examplecapture>
[Dependent]
public class CountersSpellEvent : Glyph
{
    public override Nib[] Nibs => ["a spell or ability", Prop(Controller), "counters a spell"];

    public ControlledBy Controller { get; set; }
}
