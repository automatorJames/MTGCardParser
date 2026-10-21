namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"all abilities", as what a permanent loses: "loses all abilities".</summary>
/// <exampledoc>Humble</exampledoc>
/// <examplecapture>all abilities</examplecapture>
[Dependent]
public class AllAbilities : Glyph
{
    public override Nib[] Nibs => ["all abilities"];
}
