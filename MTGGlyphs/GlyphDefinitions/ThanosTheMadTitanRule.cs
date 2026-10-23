namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thanos, the Mad Titan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thanos, the Mad Titan</exampledoc>
[TokenizationOrder(-1)]
public class ThanosTheMadTitanRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy each other creature with mana value of the chosen quality"];
}
