namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Reminder for a hybrid, Phyrexian or twobrid mana symbol, saying what pays it: "{u/p} can be paid with either {u} or 2 life", "{g/w} can be paid with either {g} or {w}", "{2/u} can be paid with any two mana or with {u}".</summary>
/// <exampledoc>Gitaxian Probe</exampledoc>
/// <examplecapture>{u/p} can be paid with either {u} or 2 life</examplecapture>
public class ManaSymbolPaymentRule : Glyph
{
    public override Nib[] Nibs => [Prop(Symbol), "can be paid with", Opt("either"), Prop(First), "or", Opt("with"), Prop(Second)];

    public ManaCost Symbol { get; set; }
    public ManaPaymentOption First { get; set; }
    public ManaPaymentOption Second { get; set; }
}
