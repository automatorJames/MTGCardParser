namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Condition on an additional choice made as a spell was cast: "the gift was promised" (gift), "evidence was collected" (collect evidence).</summary>
/// <exampledoc>Long River's Pull</exampledoc>
/// <examplecapture>the gift was promised</examplecapture>
[Dependent]
public class CastingChoiceMade : Glyph
{
    public override Nib[] Nibs => [Prop(Choice)];

    public CastingChoice Choice { get; set; }
}
