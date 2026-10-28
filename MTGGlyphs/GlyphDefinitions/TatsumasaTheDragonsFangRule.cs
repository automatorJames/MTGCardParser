namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tatsumasa, the Dragon's Fang, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tatsumasa, the Dragon's Fang</exampledoc>
[TokenizationOrder(-1)]
public class TatsumasaTheDragonsFangRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["{6}, exile tatsumasa: create a 5/5 blue dragon spirit creature token with flying"];
}
