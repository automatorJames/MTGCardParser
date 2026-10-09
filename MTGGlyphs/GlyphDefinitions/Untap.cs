namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"untap those creatures", "untap target land", "untap {this}".</summary>
/// <exampledoc>Magnetic Mountain</exampledoc>
/// <examplecapture>untap those creatures</examplecapture>
public class Untap : Glyph
{
    public override Nib[] Nibs => ["untap", Prop(Untapped)];

    public PermanentPhrase Untapped { get; set; }
}
