namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Aven Mindcensor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aven Mindcensor</exampledoc>
[TokenizationOrder(-1)]
public class AvenMindcensorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an opponent would search a library, that player searches the top four cards of that library instead"];
}
