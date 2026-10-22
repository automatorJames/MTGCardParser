namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What can't target something: "spells or abilities", "blue or black spells", "white spells or abilities from white sources", "abilities from artifact sources", "monocolored spells or abilities".</summary>
/// <exampledoc>Rebel Informer</exampledoc>
/// <examplecapture>white spells or abilities from white sources</examplecapture>
[Dependent]
public class TargetingObjects : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifier), Prop(Kinds), Prop(From)];

    [Optional]
    public OneOf<ManyOf<TargetQualifier>, TargetQualifier> Qualifier { get; set; }
    public TargetingKind Kinds { get; set; }
    [Optional]
    public FromSources From { get; set; }
}
