namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Liliana's Defeat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Liliana's Defeat</exampledoc>
[TokenizationOrder(-1)]
public class LilianasDefeatRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that permanent was a liliana planeswalker, her controller loses 3 life"];
}
