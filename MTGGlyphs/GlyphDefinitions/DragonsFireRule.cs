namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dragon's Fire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dragon's Fire</exampledoc>
[TokenizationOrder(-1)]
public class DragonsFireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as an additional cost to cast this spell, you may reveal a dragon card from your hand or choose a dragon you control"];
}
