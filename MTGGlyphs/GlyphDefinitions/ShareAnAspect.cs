namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Several objects that must have something in common with each other: "that share a card type", "that share a creature type" (Shifting Loyalties, Role Reversal).</summary>
/// <exampledoc>Shifting Loyalties</exampledoc>
/// <examplecapture>that share a card type</examplecapture>
[Dependent]
public class ShareAnAspect : Glyph
{
    public override Nib[] Nibs => ["that share", Prop(Aspect)];

    public SharedAspect Aspect { get; set; }
}
