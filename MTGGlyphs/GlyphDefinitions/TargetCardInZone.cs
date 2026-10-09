namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target creature card from your graveyard": a targeted card outside the battlefield, optionally of a type.</summary>
/// <exampledoc>Raise Dead</exampledoc>
/// <examplecapture>target creature card from your graveyard</examplecapture>
[Dependent]
public class TargetCardInZone : Glyph
{
    public override Nib[] Nibs => ["target", Prop(Qualifiers), Prop(Card)];

    [Optional]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    public CardOutsideBattlefield Card { get; set; }
}
