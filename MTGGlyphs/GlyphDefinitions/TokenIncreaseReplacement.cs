namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A replacement effect that increases tokens as they're created: "if one or more tokens would be created under your control, twice that many of those tokens are created instead", "if an effect would create one or more tokens under your control, it creates twice that many of those tokens instead" (Anointed Procession, Parallel Lives).</summary>
/// <exampledoc>Anointed Procession</exampledoc>
/// <examplecapture>if an effect would create one or more tokens under your control, it creates twice that many of those tokens instead</examplecapture>
public class TokenIncreaseReplacement : Glyph, IPredicate
{
    public override Nib[] Nibs => ["if", Alt("one or more tokens would be created", "an effect would create one or more tokens"), Prop(Control), ",", Opt("it creates"), Prop(Increase), "of those tokens", Opt("are created"), "instead"];

    public UnderControlOf Control { get; set; }
    public CounterIncrease Increase { get; set; }
}
