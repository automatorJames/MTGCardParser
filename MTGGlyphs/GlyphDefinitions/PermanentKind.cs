namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What kind of permanent something is: a card type ("land"), a land type ("island"), a creature type ("wall"), a planeswalker type ("jace", "each garruk you control"), a group of creature types ("outlaw"), another subtype ("saga"), a supertype used as a noun ("legendaries"), or a token ("a jace token you control", "tokens you control").</summary>
/// <exampledoc>Ice Storm</exampledoc>
/// <examplecapture>land</examplecapture>
[Dependent]
public class PermanentKind : GlyphOneOf
{
    public CardType? CardType { get; set; }
    public LandType? LandType { get; set; }
    public CreatureType? CreatureType { get; set; }
    public PlaneswalkerType? PlaneswalkerType { get; set; }
    public CreatureTypeGroup? TypeGroup { get; set; }
    public PermanentSubtype? Subtype { get; set; }
    public SupertypeNoun? Supertype { get; set; }
    public TokenWord? Token { get; set; }
}
