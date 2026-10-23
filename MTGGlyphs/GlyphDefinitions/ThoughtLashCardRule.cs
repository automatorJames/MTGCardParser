namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thought Lash, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thought Lash</exampledoc>
[TokenizationOrder(-1)]
public class ThoughtLashCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when a player doesn't pay this enchantment's cumulative upkeep, that player exiles all cards from their library"];
}
