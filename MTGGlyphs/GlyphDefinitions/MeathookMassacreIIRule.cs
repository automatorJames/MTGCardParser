namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Meathook Massacre II, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Meathook Massacre II</exampledoc>
[TokenizationOrder(-1)]
public class MeathookMassacreIIRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, return that card under your control with a finality counter on it"];
}
