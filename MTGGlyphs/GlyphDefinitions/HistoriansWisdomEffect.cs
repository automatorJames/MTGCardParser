namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Historian's Wisdom, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Historian's Wisdom</exampledoc>
[Dependent]
public class HistoriansWisdomEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if enchanted permanent is a creature with the greatest power among creatures on the battlefield, draw a card"];
}
