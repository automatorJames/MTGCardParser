namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent being a type, set or added by a static effect: "is an island", "is a black zombie in addition to its other colors and types", "are artifacts in addition to their other types", "is a nightmare in addition to its other creature types".</summary>
/// <exampledoc>Ghoulflesh</exampledoc>
/// <examplecapture>is a black zombie in addition to its other colors and types</examplecapture>
[Dependent]
public class IsOfType : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Kind), Prop(InAddition)];

    public TypeCopula Verb { get; set; }
    public OneOf<ColoredKind, PluralKind> Kind { get; set; }
    [RegexPattern("in addition to (its|their) other (colors and types|creature types|types)")]
    public bool InAddition { get; set; }
}
