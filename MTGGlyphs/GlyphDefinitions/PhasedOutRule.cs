namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Phased-out permanents' reminder text, singular or plural: "while it's phased out, it's treated as though it doesn't exist", "while they're phased out, they're treated as though they don't exist".</summary>
/// <exampledoc>Mist Dragon</exampledoc>
/// <examplecapture>while it's phased out, it's treated as though it doesn't exist</examplecapture>
public class PhasedOutRule : Glyph
{
    public override Nib[] Nibs => ["while", Prop(Phased), "phased out,", Pattern("(it's|they're)"), "treated as though", Pattern("(it doesn't|they don't)"), "exist"];

    public PhasedOutSubject Phased { get; set; }
}
