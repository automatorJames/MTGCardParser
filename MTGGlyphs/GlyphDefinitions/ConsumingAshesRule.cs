namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Consuming Ashes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Consuming Ashes</exampledoc>
[TokenizationOrder(-1)]
public class ConsumingAshesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it had mana value 3 or less, surveil 2"];
}
