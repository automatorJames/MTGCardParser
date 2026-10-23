namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Johan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Johan</exampledoc>
[TokenizationOrder(-1)]
public class JohanRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, attacking doesn't cause creatures you control to tap this combat if", Nib.This, "is untapped"];
}
