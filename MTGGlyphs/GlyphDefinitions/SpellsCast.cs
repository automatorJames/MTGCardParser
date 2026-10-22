namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"instant and sorcery spells you cast", "noncreature spells", "creature spells with flying you cast", "spells your opponents cast", "dragon spells you cast", "creature spells of the chosen type", "spells you cast of the chosen type", "spells your opponents cast that target this creature": spells of a kind, optionally by who casts them and what they target.</summary>
/// <exampledoc>Baral, Chief of Compliance</exampledoc>
/// <examplecapture>instant and sorcery spells you cast</examplecapture>
[Dependent]
public class SpellsCast : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Plural("spell"), Prop(With), Prop(Caster), Prop(Chosen), Prop(Targeting)];

    [Optional]
    public CompoundOf<QualifierWords> Qualifiers { get; set; }
    [Optional]
    public OneOf<WithQualifier, ChosenAspect?> With { get; set; }
    [Optional]
    public SpellCaster Caster { get; set; }
    [Optional]
    public ChosenAspect? Chosen { get; set; }
    [Optional]
    public ThatTarget Targeting { get; set; }
}
