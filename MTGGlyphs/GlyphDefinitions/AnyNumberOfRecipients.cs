namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"any number of targets", "any number of target creatures", "any number of target attacking or blocking creatures": as many recipients as the caster likes.</summary>
/// <exampledoc>Pyrotechnics</exampledoc>
/// <examplecapture>any number of targets</examplecapture>
[Dependent]
public class AnyNumberOfRecipients : Glyph
{
    public override Nib[] Nibs => ["any number of", Prop(Recipients)];

    public OneOf<TargetsOfKind, DividedRecipients?> Recipients { get; set; }
}
