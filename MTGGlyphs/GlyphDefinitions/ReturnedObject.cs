namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a return effect moves: a targeted card in a zone, a card named by where it is ("this card from your graveyard"), a particular permanent, or any one of a kind.</summary>
/// <exampledoc>Raise Dead</exampledoc>
/// <examplecapture>target creature card from your graveyard</examplecapture>
[Dependent]
public class ReturnedObject : GlyphOneOf
{
    public TargetCardInZone TargetCardInZone { get; set; }
    public PermanentFromZone PermanentFromZone { get; set; }
    public PermanentPhrase PermanentPhrase { get; set; }
    public IndefinitePermanent IndefinitePermanent { get; set; }
}
