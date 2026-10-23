namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Moonlit Meditation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moonlit Meditation</exampledoc>
[TokenizationOrder(-1)]
public class MoonlitMeditationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the first time you would create one or more tokens each turn, you may instead create that many tokens that are copies of enchanted permanent"];
}
