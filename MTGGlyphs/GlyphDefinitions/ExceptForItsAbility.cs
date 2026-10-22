namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"except for its epic ability": an ability of the original that a copy doesn't get.</summary>
/// <exampledoc>Endless Swarm</exampledoc>
/// <examplecapture>except for its epic ability</examplecapture>
[Dependent]
public class ExceptForItsAbility : Glyph
{
    public override Nib[] Nibs => ["except for its", Prop(Keyword), "ability"];

    public Keyword Keyword { get; set; }
}
