namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Elixir, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elixir</exampledoc>
[TokenizationOrder(-1)]
public class ElixirRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain life equal to the number of cards shuffled into your library this way"];
}
