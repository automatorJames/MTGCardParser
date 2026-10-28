namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tatsumasa, the Dragon's Fang, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tatsumasa, the Dragon's Fang</exampledoc>
[TokenizationOrder(-1)]
public class TatsumasaTheDragonsFangRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return tatsumasa to the battlefield under its owner's control when that token dies"];
}
