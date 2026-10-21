namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"cast using teamwork": the keyword a spell was cast with.</summary>
/// <exampledoc>Cruel Alliance</exampledoc>
/// <examplecapture>cast using teamwork</examplecapture>
[Dependent]
public class CastUsingKeyword : Glyph
{
    public override Nib[] Nibs => ["cast using", Prop(Keyword)];

    public Keyword Keyword { get; set; }
}
