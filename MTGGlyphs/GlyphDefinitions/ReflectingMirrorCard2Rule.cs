namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Reflecting Mirror, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reflecting Mirror</exampledoc>
[TokenizationOrder(-1)]
public class ReflectingMirrorCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["x is twice the mana value of that spell"];
}
