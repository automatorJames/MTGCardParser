namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"instant and sorcery spells you cast", "noncreature spells", "creature spells with flying you cast", "spells your opponents cast", "dragon spells you cast": spells of a kind, optionally by who casts them.</summary>
/// <exampledoc>Baral, Chief of Compliance</exampledoc>
/// <examplecapture>instant and sorcery spells you cast</examplecapture>
[Dependent]
public class SpellsCast : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Plural("spell"), Prop(With), Prop(Caster)];

    [Optional]
    public OneOf<ManyOf<TargetQualifier>, CompoundOf<TargetQualifier>, QualifierWords> Qualifiers { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
    [Optional]
    public SpellCaster Caster { get; set; }
}
