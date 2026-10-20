namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Phyrexian mana's reminder: "{u/p} can be paid with either {u} or 2 life".</summary>
/// <exampledoc>Gitaxian Probe</exampledoc>
/// <examplecapture>{u/p} can be paid with either {u} or 2 life</examplecapture>
public class PhyrexianManaRule : Glyph
{
    public override Nib[] Nibs => [Prop(Symbol), "can be paid with either", Prop(Mana), "or 2 life"];

    public ManaCost Symbol { get; set; }
    public ManaCost Mana { get; set; }
}
