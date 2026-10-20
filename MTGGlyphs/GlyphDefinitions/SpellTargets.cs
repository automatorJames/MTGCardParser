namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that targets this creature", "that targets a creature", "that targets a permanent you control": a spell aimed at something.</summary>
/// <exampledoc>Heroes of the Revel</exampledoc>
/// <examplecapture>that targets this creature</examplecapture>
[Dependent]
public class SpellTargets : Glyph
{
    public override Nib[] Nibs => ["that targets", Prop(Targeted)];

    public OneOf<PermanentPhrase, Permanents, IndefinitePermanent> Targeted { get; set; }
}
