namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mangara, the Diplomat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mangara, the Diplomat</exampledoc>
[Dependent]
public class MangaraTheDiplomatEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if two or more of those creatures are attacking you and/or planeswalkers you control, draw a card"];
}
