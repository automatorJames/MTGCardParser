namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Agadeem's Awakening // Agadeem, the Undercrypt, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Agadeem's Awakening // Agadeem, the Undercrypt</exampledoc>
[TokenizationOrder(-1)]
public class AgadeemsAwakeningRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return from your graveyard to the battlefield any number of target creature cards that each have a different mana value x or less"];
}
