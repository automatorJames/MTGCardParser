namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Tapping or untapping one permanent: "tap target wall", "untap {this}", "untap those creatures". Replaces the untap-only glyph.</summary>
/// <exampledoc>Ali Baba</exampledoc>
/// <examplecapture>tap target wall</examplecapture>
public class TapOrUntap : Glyph
{
    public override Nib[] Nibs => [Prop(Action), Prop(Permanent)];

    public TapAction Action { get; set; }
    public PermanentPhrase Permanent { get; set; }
}
