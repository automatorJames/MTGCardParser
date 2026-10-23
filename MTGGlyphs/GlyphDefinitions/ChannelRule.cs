namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Channel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Channel</exampledoc>
[TokenizationOrder(-1)]
public class ChannelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, any time you could activate a mana ability, you may pay 1 life"];
}
