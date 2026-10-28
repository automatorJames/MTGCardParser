namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mangara, the Diplomat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mangara, the Diplomat</exampledoc>
[TokenizationOrder(-1)]
public class MangaraTheDiplomatRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an opponent attacks with creatures, if two or more of those creatures are attacking you and/or planeswalkers you control, draw a card"];
}
