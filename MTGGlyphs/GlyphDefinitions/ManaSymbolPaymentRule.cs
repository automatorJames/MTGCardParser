namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Reminder for a hybrid, Phyrexian or twobrid mana symbol, saying what pays it: "{u/p} can be paid with either {u} or 2 life", "{g/w} can be paid with either {g} or {w}", "{r/w/p} can be paid with {r}, {w}, or 2 life", "{2/u} can be paid with any two mana or with {u}".</summary>
/// <exampledoc>Gitaxian Probe</exampledoc>
/// <examplecapture>{u/p} can be paid with either {u} or 2 life</examplecapture>
public class ManaSymbolPaymentRule : Glyph
{
    public override Nib[] Nibs => [Prop(Symbol), "can be paid with", Opt("either"), Prop(Options)];

    public ManaCost Symbol { get; set; }
    public OneOf<TwobridPayment, ManyOf<ManaPaymentOption>> Options { get; set; }
}
