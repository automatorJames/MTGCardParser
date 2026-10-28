namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with ward {2}", "with protection from black", "with toxic 1", "with cumulative upkeep {1}": one keyword ability, possibly with a parameter, that a token or a face-down creature is made with. Plain keywords ("with flying", "with flying or reach") are <see cref="WithKeywords"/>.</summary>
/// <exampledoc>Ethereal Investigator</exampledoc>
/// <examplecapture>with ward {2}</examplecapture>
[Dependent]
public class WithKeywordAbility : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Ability)];

    public KeywordAbility Ability { get; set; }
}
