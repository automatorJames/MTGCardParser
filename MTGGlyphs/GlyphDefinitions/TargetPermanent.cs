namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "target creature", "target nonartifact, nonblack creature": a targeted card on the battlefield. Every match is a
/// referent, so a later "it" refers to the card targeted - "destroy target wall. it can't be regenerated."
/// </summary>
[Dependent]
[Introduces(GrammaticalNumber.Singular)]
public class TargetPermanent : Glyph
{
    public override Nib[] Nibs => ["target", Prop(Qualifiers), Prop(CardOrCreatureType)];

    [Optional]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }

    public CardOrCreatureTypeTarget CardOrCreatureType { get; set; }
}
