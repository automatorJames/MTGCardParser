namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Errant Minion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Errant Minion</exampledoc>
/// <examplecapture>prevent x of that damage, where x is the amount of mana that player paid this way</examplecapture>
[TokenizationOrder(-1)]
public class ErrantMinionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent x of that damage, where x is the amount of mana that player paid this way"];
}
