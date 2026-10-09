namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a red source of your choice", "an artifact source of your choice": a source of damage the player picks, optionally of a color or type.</summary>
/// <exampledoc>Circle of Protection: Red</exampledoc>
/// <examplecapture>a red source of your choice</examplecapture>
[Dependent]
public class ChosenSource : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Qualifiers), "source of your choice"];

    [Optional]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
}
