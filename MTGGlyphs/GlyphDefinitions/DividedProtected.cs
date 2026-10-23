namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Targets that share prevented damage: "any number of targets, divided as you choose" (Remedy).</summary>
/// <exampledoc>Remedy</exampledoc>
/// <examplecapture>any number of targets, divided as you choose</examplecapture>
[Dependent]
public class DividedProtected : Glyph
{
    public override Nib[] Nibs => [Prop(Recipients), ", divided", Prop(Division)];

    public AnyNumberOfRecipients Recipients { get; set; }
    public DamageDivision Division { get; set; }
}
