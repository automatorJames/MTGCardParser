namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that aren't mana abilities", "that aren't mana abilities or loyalty abilities": the abilities split second and its kin stop.</summary>
/// <exampledoc>Extirpate</exampledoc>
/// <examplecapture>that aren't mana abilities</examplecapture>
[Dependent]
public class NonManaAbilities : Glyph
{
    public override Nib[] Nibs => ["that aren't mana abilities", Prop(OrLoyalty)];

    [Optional]
    [RegexPattern("or loyalty abilities")]
    public bool OrLoyalty { get; set; }
}
