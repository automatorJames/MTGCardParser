namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent being a type, set or added by a static effect: "is an island", "is a black zombie in addition to its other colors and types", "are artifacts in addition to their other types", "is a nightmare in addition to its other creature types", "is the chosen type in addition to its other types", "is every basic land type in addition to its other types", "is a swamp in addition to its other land types", "is a spider hero in addition to its other types", "is a legendary soldier in addition to its other types" - and for how long, when it says: "is an island in addition to its other types for as long as it has a flood counter on it".</summary>
/// <exampledoc>Ghoulflesh</exampledoc>
/// <examplecapture>is a black zombie in addition to its other colors and types</examplecapture>
[Dependent]
public class IsOfType : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Legendary), Prop(Kind), Prop(InAddition), Prop(Duration)];

    public TypeCopula Verb { get; set; }
    [Optional]
    [RegexPattern("legendary")]
    public bool? Legendary { get; set; }
    public OneOf<ColoredKind, PluralKind, TypeDesignation?, DescribedKinds> Kind { get; set; }
    [RegexPattern("in addition to (its|their) other (colors and types|creature types|land types|types|colors)")]
    public bool InAddition { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
