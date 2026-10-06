namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"untap those creatures".</summary>
/// <exampledoc>Magnetic Mountain</exampledoc>
/// <examplecapture>untap those creatures</examplecapture>
public class Untap : Glyph
{
    public override Nib[] Nibs => ["untap", Prop(Untapped)];

    public ThatCard Untapped { get; set; }
}
