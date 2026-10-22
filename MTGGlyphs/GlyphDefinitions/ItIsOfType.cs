namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it's a dragon in addition to its other types", "it's a zombie in addition to its other creature types", "it's an island in addition to its other types for as long as it has a flood counter on it": the object just named gaining types, with the verb run into "it" - so it can't be a subject and an <see cref="IsOfType"/> predicate. The "in addition" is required, so a bare "it's a creature" stays a condition (<see cref="CardIsKind"/>).</summary>
/// <exampledoc>Draconic Destiny</exampledoc>
/// <examplecapture>it's a dragon in addition to its other types</examplecapture>
public class ItIsOfType : Glyph
{
    public override Nib[] Nibs => [Prop(It), Pattern("an?"), Prop(Kind), "in addition to its other", Prop(Scope), Prop(Duration)];

    public ItIs It { get; set; }
    public OneOf<ColoredKind, PluralKind, OneOf<TypeDesignation?, DescribedKinds>> Kind { get; set; }
    public OtherTypesScope Scope { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
