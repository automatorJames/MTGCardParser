namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent staying as it is: "{this} remains tapped", "{this} remains on the battlefield", "it remains exiled".</summary>
/// <exampledoc>Ice Floe</exampledoc>
/// <examplecapture>{this} remains tapped</examplecapture>
[Dependent]
public class Remains : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), "remains", Prop(State)];

    public PermanentPhrase Subject { get; set; }
    public RemainingState State { get; set; }
}
