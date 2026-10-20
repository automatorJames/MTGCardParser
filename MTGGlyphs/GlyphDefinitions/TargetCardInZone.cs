namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target creature card from your graveyard", "up to two target cards from a single graveyard": a targeted card (or several) outside the battlefield, optionally of a type or a choice of types.</summary>
/// <exampledoc>Raise Dead</exampledoc>
/// <examplecapture>target creature card from your graveyard</examplecapture>
[Dependent]
public class TargetCardInZone : Glyph
{
    public override Nib[] Nibs => [Prop(UpTo), "target", Prop(Qualifiers), Prop(Card)];

    [Optional]
    public UpToCount UpTo { get; set; }
    [Optional]
    public OneOf<ManyOf<TargetQualifier>, CompoundOf<TargetQualifier>> Qualifiers { get; set; }
    public CardOutsideBattlefield Card { get; set; }
}
