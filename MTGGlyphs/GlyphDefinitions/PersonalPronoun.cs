namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"he", "she", "him", "her" as an object: a legendary character's pronoun standing for the card itself, as "it" does for other cards ("prevent all damage that would be dealt to him this turn", "he enters tapped and attacking"). A back-reference to the card named earlier in the line.</summary>
/// <exampledoc>Gideon Jura</exampledoc>
/// <examplecapture>him</examplecapture>
[Dependent]
public class PersonalPronoun : BackReference
{
    public override Nib[] Nibs => [Pattern(@"(he|she|him|her)\b(?!')")];
}
