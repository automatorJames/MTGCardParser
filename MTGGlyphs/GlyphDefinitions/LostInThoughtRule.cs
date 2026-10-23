namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lost in Thought, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lost in Thought</exampledoc>
[TokenizationOrder(-1)]
public class LostInThoughtRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its controller may exile three cards from their graveyard for that player to ignore this effect until end of turn"];
}
