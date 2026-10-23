namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Torrent of Lava, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Torrent of Lava</exampledoc>
/// <examplecapture>as long as {this} is on the stack, each creature has "{t}: prevent the next 1 damage that would be dealt to this creature by {this} this turn</examplecapture>
[TokenizationOrder(-1)]
public class TorrentOfLavaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as", Nib.This, "is on the stack, each creature has \"{t}: prevent the next 1 damage that would be dealt to this creature by", Nib.This, "this turn"];
}
