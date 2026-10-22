namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent or card staying as it is: "{this} remains tapped", "{this} remains on the battlefield", "it remains exiled", "they remain exiled".</summary>
/// <exampledoc>Ice Floe</exampledoc>
/// <examplecapture>{this} remains tapped</examplecapture>
[Dependent]
public class Remains : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Pattern("remains?"), Prop(State)];

    public OneOf<PermanentPhrase, They> Subject { get; set; }
    public RemainingState State { get; set; }
}
