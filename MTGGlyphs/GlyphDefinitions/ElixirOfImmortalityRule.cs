namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Elixir of Immortality, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elixir of Immortality</exampledoc>
[TokenizationOrder(-1)]
public class ElixirOfImmortalityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["shuffle this artifact and your graveyard into their owner's library"];
}
