namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of War Historian, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>War Historian</exampledoc>
[TokenizationOrder(-1)]
public class WarHistorianRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has indestructible as long as it attacked a battle this turn"];
}
