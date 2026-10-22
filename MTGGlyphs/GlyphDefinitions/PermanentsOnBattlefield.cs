namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Permanents of a kind existing anywhere on the battlefield, as a condition: "there is a mountain on the battlefield", "there are five or more islands on the battlefield", "there are no zombies on the battlefield", "there's another creature on the battlefield".</summary>
/// <exampledoc>Harbor Serpent</exampledoc>
/// <examplecapture>there are five or more islands on the battlefield</examplecapture>
[Dependent]
public class PermanentsOnBattlefield : Glyph
{
    public override Nib[] Nibs => [Alt("there are", "there is", "there's"), Prop(What), Prop(OnBattlefield)];

    public OneOf<PermanentsThreshold, NoPermanents, IndefinitePermanent> What { get; set; }
    [RegexPattern("on the battlefield")]
    public bool OnBattlefield { get; set; }
}
