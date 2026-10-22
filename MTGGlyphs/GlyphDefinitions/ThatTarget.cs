namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that target this creature", "that target you": spells picked out by what they target.</summary>
/// <exampledoc>Icefall Regent</exampledoc>
/// <examplecapture>that target this creature</examplecapture>
[Dependent]
public class ThatTarget : Glyph
{
    public override Nib[] Nibs => ["that target", Prop(Target)];

    public OneOf<PermanentPhrase, WhichPlayer?> Target { get; set; }
}
