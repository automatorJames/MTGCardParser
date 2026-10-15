namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Tapping or untapping permanents: "tap target wall", "untap {this}", "untap those creatures", "tap x target creatures", or as a cost, "tap enchanted creature".</summary>
/// <exampledoc>Ali Baba</exampledoc>
/// <examplecapture>tap target wall</examplecapture>
public class TapOrUntap : Glyph
{
    public override Nib[] Nibs => [Prop(Action), Prop(Permanent)];

    public TapAction Action { get; set; }
    public OneOf<PermanentPhrase, IndefinitePermanent, CountedTargets> Permanent { get; set; }
}
