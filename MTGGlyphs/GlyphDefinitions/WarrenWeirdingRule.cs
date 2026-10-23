namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Warren Weirding, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Warren Weirding</exampledoc>
/// <examplecapture>if a goblin is sacrificed this way, that player creates two 1/1 black goblin rogue creature tokens, and those tokens gain haste until end of turn</examplecapture>
[TokenizationOrder(-1)]
public class WarrenWeirdingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a goblin is sacrificed this way, that player creates two 1/1 black goblin rogue creature tokens, and those tokens gain haste until end of turn"];
}
