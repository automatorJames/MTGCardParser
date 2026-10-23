namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Faerie Fencing, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Faerie Fencing</exampledoc>
[TokenizationOrder(-1)]
public class FaerieFencingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature gets an additional -3/-3 until end of turn if you controlled a faerie as you cast this spell"];
}
