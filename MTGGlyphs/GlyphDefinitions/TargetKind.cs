namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What can be targeted: a player or opponent, a card type, a creature type, or a token.</summary>
/// <exampledoc>Ancestral Recall</exampledoc>
/// <examplecapture>player</examplecapture>
[Dependent]
public class TargetKind : GlyphOneOf
{
    public PlayerIdentity? PlayerIdentity { get; set; }
    public CardType? CardType { get; set; }
    public CreatureType? CreatureType { get; set; }
    public TokenNoun? Token { get; set; }
}
