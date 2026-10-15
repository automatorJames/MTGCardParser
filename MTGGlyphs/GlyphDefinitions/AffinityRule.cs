namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Affinity for artifacts, with its reminder text.</summary>
/// <exampledoc>Assert Authority</exampledoc>
/// <examplecapture>this spell costs {1} less to cast for each artifact you control</examplecapture>
public class AffinityRule : Glyph
{
    public override Nib[] Nibs => ["this spell costs {1} less to cast for each artifact you control"];
}
