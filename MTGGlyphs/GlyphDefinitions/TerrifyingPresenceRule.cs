namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Terrifying Presence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Terrifying Presence</exampledoc>
[TokenizationOrder(-1)]
public class TerrifyingPresenceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all combat damage that would be dealt by creatures other than target creature this turn"];
}
