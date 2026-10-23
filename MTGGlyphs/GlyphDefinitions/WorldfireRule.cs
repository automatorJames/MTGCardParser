namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Worldfire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Worldfire</exampledoc>
[TokenizationOrder(-1)]
public class WorldfireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all cards from all hands and graveyards"];
}
