namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sentinel of Lost Lore, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sentinel of Lost Lore</exampledoc>
[TokenizationOrder(-1)]
public class SentinelOfLostLoreRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• return target card you own in exile that has an adventure to your hand"];
}
