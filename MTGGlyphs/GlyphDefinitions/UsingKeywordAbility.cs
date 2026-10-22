namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Casting by way of a keyword's alternative cost: "using its warp ability", "using its mutate ability", "using their sneak abilities".</summary>
/// <exampledoc>Tenacious Underdog</exampledoc>
/// <examplecapture>using its blitz ability</examplecapture>
[Dependent]
public class UsingKeywordAbility : Glyph
{
    public override Nib[] Nibs => ["using", Alt("its", "their"), Prop(Keyword), Pattern("abilit(y|ies)")];

    public Keyword Keyword { get; set; }
}
