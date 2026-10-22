namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword with a mana cost and the power and toughness it gives, written "cost — P/T": "prototype {1}{w}{w} — 3/3".</summary>
/// <exampledoc>Steel Seraph</exampledoc>
/// <examplecapture>prototype {1}{w}{w} — 3/3</examplecapture>
[Dependent]
public class KeywordWithCostAndStats : Glyph
{
    public override Nib[] Nibs => [Prop(Keyword), Prop(Cost), "—", Prop(Stats)];

    public Keyword Keyword { get; set; }
    public ManaCost Cost { get; set; }
    public PowerToughness Stats { get; set; }
}
