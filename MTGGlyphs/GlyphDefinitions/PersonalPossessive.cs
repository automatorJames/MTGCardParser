namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A legend's gendered possessive pronoun ("his", "her") referring back to the named permanent, as in "his power" or "her power".</summary>
/// <exampledoc>Stature, Size Shifter</exampledoc>
/// <examplecapture>her</examplecapture>
[Dependent]
public class PersonalPossessive : BackReference
{
    public override Nib[] Nibs => [Pattern("(his|her)")];
}
