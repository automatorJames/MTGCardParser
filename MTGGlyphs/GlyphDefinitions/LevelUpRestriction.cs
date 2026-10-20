namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword's timing restriction in reminder text: "level up only as a sorcery", "unearth only as a sorcery", "plot only as a sorcery". (Named for its first use; it now takes any keyword.)</summary>
/// <exampledoc>Joraga Treespeaker</exampledoc>
/// <examplecapture>level up only as a sorcery</examplecapture>
public class LevelUpRestriction : Glyph
{
    public override Nib[] Nibs => [Prop(Keyword), "only as a sorcery"];

    public Keyword Keyword { get; set; }
}
