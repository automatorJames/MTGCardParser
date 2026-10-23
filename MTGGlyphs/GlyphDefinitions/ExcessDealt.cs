namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"dealt this way", "dealt to that creature this way": excess damage tied to the damage effect just described, and to what it was dealt to.</summary>
/// <exampledoc>Torch the Witness</exampledoc>
/// <examplecapture>dealt to that creature this way</examplecapture>
[Dependent]
public class ExcessDealt : Glyph
{
    public override Nib[] Nibs => ["dealt", Prop(To), "this way"];

    [Optional]
    public DamagedPermanentRef To { get; set; }
}
