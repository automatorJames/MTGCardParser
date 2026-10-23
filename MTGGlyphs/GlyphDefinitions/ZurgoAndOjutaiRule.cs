namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Zurgo and Ojutai, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zurgo and Ojutai</exampledoc>
[TokenizationOrder(-1)]
public class ZurgoAndOjutaiRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may return one of those dragons to its owner's hand"];
}
