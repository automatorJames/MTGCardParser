namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Airlift Chaplain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Airlift Chaplain</exampledoc>
/// <examplecapture>you may put a plains card or a creature card with mana value 3 or less from among the cards milled this way into your hand</examplecapture>
[TokenizationOrder(-1)]
public class AirliftChaplainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put a plains card or a creature card with mana value 3 or less from among the cards milled this way into your hand"];
}
