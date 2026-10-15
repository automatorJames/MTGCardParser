namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"to and dealt by that creature": damage both to and from one permanent, as when a creature is removed from combat in effect (Maze of Ith).</summary>
/// <exampledoc>Maze of Ith</exampledoc>
/// <examplecapture>to and dealt by that creature</examplecapture>
[Dependent]
public class DealtToAndBy : Glyph
{
    public override Nib[] Nibs => ["to and dealt by", Prop(Who)];

    public PermanentPhrase Who { get; set; }
}
