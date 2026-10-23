namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fatal Push, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fatal Push</exampledoc>
/// <examplecapture>revolt — destroy that creature if it has mana value 4 or less instead if a permanent left the battlefield under your control this turn</examplecapture>
[TokenizationOrder(-1)]
public class FatalPushRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["revolt — destroy that creature if it has mana value 4 or less instead if a permanent left the battlefield under your control this turn"];
}
