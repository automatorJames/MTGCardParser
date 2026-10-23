namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Break the Spell, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Break the Spell</exampledoc>
[TokenizationOrder(-1)]
public class BreakTheSpellRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a permanent you controlled or a token was destroyed this way, draw a card"];
}
