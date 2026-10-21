namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A blocking requirement (lure): "all creatures able to block this creature do so", "all creatures able to block target creature this turn do so", "all walls able to block this creature do so".</summary>
/// <exampledoc>Lure</exampledoc>
/// <examplecapture>all creatures able to block enchanted creature do so</examplecapture>
public class MustBlockAll : Glyph
{
    public override Nib[] Nibs => ["all", Prop(Blockers), "able to block", Prop(Attacker), Prop(Duration), "do so"];

    public Permanents Blockers { get; set; }
    public OneOf<EnchantedPermanent, PermanentPhrase> Attacker { get; set; }
    [Optional]
    public ThisPeriod Duration { get; set; }
}
