namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chandra's Incinerator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra's Incinerator</exampledoc>
[TokenizationOrder(-1)]
public class ChandrasIncineratorRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a source you control deals noncombat damage to an opponent, this creature deals that much damage to target creature or planeswalker that player controls"];
}
