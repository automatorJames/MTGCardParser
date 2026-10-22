namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A token's one keyword with a parameter: "with protection from black", "with toxic 1".</summary>
/// <exampledoc>Viral Spawning</exampledoc>
/// <examplecapture>with toxic 1</examplecapture>
[Dependent]
public class WithParameterizedKeyword : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Ability)];

    public OneOf<Protection, KeywordWithAmount> Ability { get; set; }
}
