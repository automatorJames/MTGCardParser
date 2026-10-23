namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Plated Pegasus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Plated Pegasus</exampledoc>
[TokenizationOrder(-1)]
public class PlatedPegasusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a spell would deal damage to a permanent or player, prevent 1 damage that spell would deal to that permanent or player"];
}
