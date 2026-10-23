namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Darksteel Sentinel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Darksteel Sentinel</exampledoc>
[TokenizationOrder(-1)]
public class DarksteelSentinelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if its toughness is 0 or less, it's still put into its owner's graveyard"];
}
