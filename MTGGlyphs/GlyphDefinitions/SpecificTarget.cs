namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "target creature", "target nonartifact, nonblack creature", "target player": "target" and what it targets. What
/// it targets is a referent, so a later "it" refers to it - "destroy target wall. it can't be regenerated." - and so
/// does a later "that creature", "that wall" or "that player", whose kind (PlayerIdentity, CardType, CreatureType) is
/// the type of what was captured.
/// </summary>
/// <exampledoc>Crumble</exampledoc>
/// <examplecapture>target artifact</examplecapture>
[Dependent]
public class SpecificTarget : Glyph
{
    public override Nib[] Nibs => ["target", Prop(Qualifiers), Prop(Kind)];

    [Optional]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }

    [Referent]
    [Singular]
    public TargetKind Kind { get; set; }
}
