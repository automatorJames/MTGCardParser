namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Void, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Void</exampledoc>
[TokenizationOrder(-1)]
public class VoidCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all artifacts and creatures with mana value equal to that number"];
}
