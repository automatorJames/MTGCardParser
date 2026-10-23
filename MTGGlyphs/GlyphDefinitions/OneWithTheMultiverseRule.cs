namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of One with the Multiverse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>One with the Multiverse</exampledoc>
[TokenizationOrder(-1)]
public class OneWithTheMultiverseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once during each of your turns, you may cast a spell from your hand or the top of your library without paying its mana cost"];
}
