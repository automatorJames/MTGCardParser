namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Obsidian Fireheart, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Obsidian Fireheart</exampledoc>
[TokenizationOrder(-1)]
public class ObsidianFireheartRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["(the land continues to burn after this creature has left the battlefield"];
}
