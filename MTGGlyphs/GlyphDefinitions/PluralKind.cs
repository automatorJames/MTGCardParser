namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A kind of permanent named in the plural: "creatures", "lands", "nonbasic lands" - one of a list in "artifacts, creatures, and lands".</summary>
/// <exampledoc>Jokulhaups</exampledoc>
/// <examplecapture>creatures</examplecapture>
[Dependent]
public class PluralKind : Glyph
{
    public override Nib[] Nibs => [Plural(Prop(Kind))];

    public PermanentKind Kind { get; set; }
}
