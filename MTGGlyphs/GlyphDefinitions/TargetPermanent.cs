namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "target creature", "target nonartifact, nonblack creature": a targeted card on the battlefield. What it targets is
/// a referent, so a later "it" refers to it - "destroy target wall. it can't be regenerated." - and so does a later
/// "that creature" or "that wall", whose kind (CardType, CreatureType) is the type of what was captured.
/// </summary>
/// <exampledoc>Crumble</exampledoc>
/// <examplecapture>target artifact</examplecapture>
[Dependent]
public class TargetPermanent : Glyph
{
    public override Nib[] Nibs => ["target", Prop(Qualifiers), Prop(CardOrCreatureType)];

    [Optional]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }

    [Referent(GrammaticalNumber.Singular)]
    public CardOrCreatureTypeTarget CardOrCreatureType { get; set; }
}
