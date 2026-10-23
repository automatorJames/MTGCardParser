namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Karplusan Minotaur, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Karplusan Minotaur</exampledoc>
[TokenizationOrder(-1)]
public class KarplusanMinotaurRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["cumulative upkeep—flip a coin"];
}
