namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A type a choice of type excludes: "other than wall", "other than creature or land".</summary>
/// <exampledoc>Standardize</exampledoc>
/// <examplecapture>other than wall</examplecapture>
[Dependent]
public class ExcludedKind : Glyph
{
    public override Nib[] Nibs => ["other than", Prop(Kinds)];

    public OneOf<ManyOf<PermanentKind>, PermanentKind> Kinds { get; set; }
}
