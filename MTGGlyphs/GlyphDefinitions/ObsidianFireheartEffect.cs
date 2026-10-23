namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Obsidian Fireheart, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Obsidian Fireheart</exampledoc>
[Dependent]
public class ObsidianFireheartEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a blaze counter on target land without a blaze counter on it"];
}
