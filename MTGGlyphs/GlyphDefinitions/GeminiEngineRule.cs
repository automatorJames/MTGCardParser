namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gemini Engine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gemini Engine</exampledoc>
[TokenizationOrder(-1)]
public class GeminiEngineRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its power is equal to this creature's power and its toughness is equal to this creature's toughness"];
}
