namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"other artifacts you control", "other creatures": permanents of a kind besides this one.</summary>
/// <exampledoc>Karn, Gilded Guardian</exampledoc>
/// <examplecapture>other artifacts you control</examplecapture>
[Dependent]
public class OtherPermanents : Glyph
{
    public override Nib[] Nibs => ["other", Prop(Permanents)];

    public Permanents Permanents { get; set; }
}
