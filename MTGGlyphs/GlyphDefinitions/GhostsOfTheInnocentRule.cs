namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ghosts of the Innocent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ghosts of the Innocent</exampledoc>
[TokenizationOrder(-1)]
public class GhostsOfTheInnocentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a source would deal damage to a permanent or player, it deals half that damage, rounded down, to that permanent or player instead"];
}
