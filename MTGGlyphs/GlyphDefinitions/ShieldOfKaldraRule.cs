namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shield of Kaldra, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shield of Kaldra</exampledoc>
[TokenizationOrder(-1)]
public class ShieldOfKaldraRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["equipment named sword of kaldra,", Nib.This, ", and helm of kaldra have indestructible"];
}
