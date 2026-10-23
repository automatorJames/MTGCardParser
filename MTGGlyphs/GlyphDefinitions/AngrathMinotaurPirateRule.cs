namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Angrath, Minotaur Pirate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Angrath, Minotaur Pirate</exampledoc>
[TokenizationOrder(-1)]
public class AngrathMinotaurPirateRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["angrath deals damage to that player equal to their total power"];
}
