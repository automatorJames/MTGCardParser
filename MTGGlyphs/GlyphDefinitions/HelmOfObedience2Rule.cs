namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Helm of Obedience, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Helm of Obedience</exampledoc>
/// <examplecapture>if one or more creature cards were put into that graveyard this way, sacrifice this artifact and put one of them onto the battlefield under your control</examplecapture>
[TokenizationOrder(-1)]
public class HelmOfObedience2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if one or more creature cards were put into that graveyard this way, sacrifice this artifact and put one of them onto the battlefield under your control"];
}
