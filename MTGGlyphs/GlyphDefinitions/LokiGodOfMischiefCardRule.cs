namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Loki, God of Mischief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Loki, God of Mischief</exampledoc>
[TokenizationOrder(-1)]
public class LokiGodOfMischiefCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a player or permanent becomes the target of an ability you control, draw a card"];
}
