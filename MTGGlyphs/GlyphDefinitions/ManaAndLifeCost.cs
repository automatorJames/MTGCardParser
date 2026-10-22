namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two costs paid together: "{1} and 1 life", "{4} and 2 life", "{b} and 1 life".</summary>
/// <exampledoc>Mundungu</exampledoc>
/// <examplecapture>{1} and 1 life</examplecapture>
[Dependent]
public class ManaAndLifeCost : Glyph
{
    public override Nib[] Nibs => [Prop(Mana), "and", Prop(Life)];

    public ManaCost Mana { get; set; }
    public LifeQuantity Life { get; set; }
}
