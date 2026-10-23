namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player allowed to cast spells (and activate abilities) only at some times: "can cast spells only during your turn", "can cast spells only any time they could cast a sorcery", "can cast spells and activate abilities only during their own turns".</summary>
/// <exampledoc>Teferi, Mage of Zhalfir</exampledoc>
/// <examplecapture>can cast spells only any time they could cast a sorcery</examplecapture>
[Dependent]
public class CastOnlyWhen : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can", Prop(Verb), Prop(What), Prop(AndActivate), "only", Prop(When)];

    public PlayVerb Verb { get; set; }
    public SpellsCast What { get; set; }
    [Optional]
    [RegexPattern("and activate abilities")]
    public bool? AndActivate { get; set; }
    public OneOf<DuringPhase, SorcerySpeed> When { get; set; }
}
