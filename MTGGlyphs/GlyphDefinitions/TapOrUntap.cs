namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Tapping or untapping permanents: "tap target wall", "untap {this}", "untap those creatures", "tap x target creatures", "untap all creatures you control", or as a cost, "tap enchanted creature". Also a predicate, so "you may tap target creature" resolves.</summary>
/// <exampledoc>Ali Baba</exampledoc>
/// <examplecapture>tap target wall</examplecapture>
public class TapOrUntap : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Action), Prop(Permanent)];

    public TapAction Action { get; set; }
    public OneOf<PermanentPhrase, IndefinitePermanent, PermanentGroup> Permanent { get; set; }
}
