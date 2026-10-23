namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Yidaro, Wandering Monster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Yidaro, Wandering Monster</exampledoc>
[TokenizationOrder(-1)]
public class YidaroWanderingMonsterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you've cycled a card named", Nib.This, "four or more times this game, put it onto the battlefield from your graveyard instead"];
}
