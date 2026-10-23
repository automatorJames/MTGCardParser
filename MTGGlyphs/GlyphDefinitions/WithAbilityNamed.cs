namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with a morph ability", "with a cycling ability": a qualifier on cards or permanents having some keyword ability, named as an ability rather than given its cost.</summary>
/// <exampledoc>Rooting Moloch</exampledoc>
/// <examplecapture>with a cycling ability</examplecapture>
[Dependent]
public class WithAbilityNamed : Glyph
{
    public override Nib[] Nibs => [Prop(Having), Pattern("an?"), Prop(Keyword), "ability"];

    public Having Having { get; set; }
    public Keyword Keyword { get; set; }
}
