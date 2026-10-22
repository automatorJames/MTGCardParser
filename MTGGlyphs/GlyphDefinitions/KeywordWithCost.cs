namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword with a mana cost parameter: "cycling {2}", "equip {1}", "flashback {3}{r}", or two costs either or both of which may be paid: "kicker {2}{u} and/or {1}{b}".</summary>
/// <exampledoc>Angel of the God-Pharaoh</exampledoc>
/// <examplecapture>cycling {2}</examplecapture>
[Dependent]
public class KeywordWithCost : Glyph
{
    public override Nib[] Nibs => [Prop(Keyword), Prop(Cost), Prop(AndOr)];

    public Keyword Keyword { get; set; }
    public ManaCost Cost { get; set; }
    [Optional]
    public AndOrCost AndOr { get; set; }
}
