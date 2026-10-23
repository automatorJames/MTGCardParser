namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ziatora's Envoy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ziatora's Envoy</exampledoc>
/// <examplecapture>you may play a land from the top of your library or cast a spell with mana value less than or equal to the damage dealt from the top of your library without paying its mana cost</examplecapture>
[TokenizationOrder(-1)]
public class ZiatorasEnvoyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may play a land from the top of your library or cast a spell with mana value less than or equal to the damage dealt from the top of your library without paying its mana cost"];
}
