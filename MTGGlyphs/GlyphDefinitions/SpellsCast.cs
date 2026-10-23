namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"instant and sorcery spells you cast", "noncreature spells", "creature spells with flying you cast", "creature spells you cast with mana value 6 or greater", "spells your opponents cast", "dragon spells you cast", "artifact spells you cast this turn", "creature spells of the chosen type", "spells you cast of the chosen type", "spells your opponents cast that target this creature", "spells your opponents cast that target a merfolk you control", "spells from graveyards", "spells you cast that share a card type with the exiled card", "spells you cast this turn that are white and/or blue": spells of a kind, optionally by who casts them (and when), from where, what they target, and what they share with another object or what colors they are.</summary>
/// <exampledoc>Baral, Chief of Compliance</exampledoc>
/// <examplecapture>instant and sorcery spells you cast</examplecapture>
[Dependent]
public class SpellsCast : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Plural("spell"), Prop(With), Prop(Caster), Prop(WithAfterCaster), Prop(Chosen), Prop(From), Prop(Targeting), Prop(Sharing)];

    [Optional]
    public CompoundOf<QualifierWords> Qualifiers { get; set; }
    [Optional]
    public OneOf<WithQualifier, ChosenAspect?> With { get; set; }
    [Optional]
    public SpellCaster Caster { get; set; }
    [Optional]
    public WithQualifier WithAfterCaster { get; set; }
    [Optional]
    public ChosenAspect? Chosen { get; set; }
    [Optional]
    public CastFrom? From { get; set; }
    [Optional]
    public ThatTarget Targeting { get; set; }
    [Optional]
    public OneOf<SharesCreatureType, ThatsColored> Sharing { get; set; }
}
