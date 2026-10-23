namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Serra Paragon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Serra Paragon</exampledoc>
/// <examplecapture>once during each of your turns, you may play a land from your graveyard or cast a permanent spell with mana value 3 or less from your graveyard</examplecapture>
[TokenizationOrder(-1)]
public class SerraParagonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once during each of your turns, you may play a land from your graveyard or cast a permanent spell with mana value 3 or less from your graveyard"];
}
