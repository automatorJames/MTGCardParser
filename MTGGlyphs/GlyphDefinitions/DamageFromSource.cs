namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Damage as a thing an effect acts on, optionally by its source's color: "damage", "damage from a red source".</summary>
/// <exampledoc>Honorable Passage</exampledoc>
/// <examplecapture>damage from a red source</examplecapture>
[Dependent]
public class DamageFromSource : Glyph
{
    public override Nib[] Nibs => ["damage", Prop(Source)];

    [Optional]
    public SourceOfColor Source { get; set; }
}
