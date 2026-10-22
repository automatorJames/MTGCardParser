namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The kind of object a spell's only target is: "a single creature", "a player".</summary>
/// <exampledoc>Rebound</exampledoc>
/// <examplecapture>a player</examplecapture>
[Dependent]
public class SingleTargetKind : Glyph
{
    public override Nib[] Nibs => ["a", Opt("single"), Prop(Kind)];

    public TargetKind Kind { get; set; }
}
