namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Seize the Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Seize the Storm</exampledoc>
/// <examplecapture>create a red elemental creature token with trample and "this token's power and toughness are each equal to the number of instant and sorcery cards in your graveyard plus the number of cards with flashback you own in exile</examplecapture>
[TokenizationOrder(-1)]
public class SeizeTheStormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a red elemental creature token with trample and \"this token's power and toughness are each equal to the number of instant and sorcery cards in your graveyard plus the number of cards with flashback you own in exile"];
}
