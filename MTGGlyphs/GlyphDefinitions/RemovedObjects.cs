namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a destroy or exile effect removes when it isn't one particular permanent or a targeted card: a group ("all creatures", "up to two target creatures"), any one permanent of a kind ("a creature you control"), or any one card in a zone ("a card from your graveyard").</summary>
/// <exampledoc>Wormfang Newt</exampledoc>
/// <examplecapture>a land you control</examplecapture>
[Dependent]
public class RemovedObjects : GlyphOneOf
{
    public PermanentGroup Group { get; set; }
    public IndefiniteCardInZone IndefiniteCard { get; set; }
    public IndefinitePermanent IndefinitePermanent { get; set; }
}
