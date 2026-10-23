namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"twice each turn rather than only once", "twice this turn rather than only once": an extra activation of abilities otherwise limited to one per turn.</summary>
/// <exampledoc>Oath of Teferi</exampledoc>
/// <examplecapture>twice each turn rather than only once</examplecapture>
[Dependent]
public class ExtraActivation : Glyph
{
    public override Nib[] Nibs => ["twice", Prop(Period), "rather than only once"];

    public ActivationPeriod Period { get; set; }
}
