namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Tapping or untapping one permanent: "tap target wall", "untap {this}", "untap those creatures", or as a cost, "tap enchanted creature".</summary>
/// <exampledoc>Ali Baba</exampledoc>
/// <examplecapture>tap target wall</examplecapture>
public class TapOrUntap : Glyph
{
    public override Nib[] Nibs => [Prop(Action), Prop(Permanent)];

    public TapAction Action { get; set; }
    public OneOf<PermanentPhrase, IndefinitePermanent> Permanent { get; set; }
}
