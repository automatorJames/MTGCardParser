namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Runesword, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Runesword</exampledoc>
[TokenizationOrder(-1)]
public class RuneswordRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature dealt damage by the targeted creature would die this turn, exile that creature instead"];
}
