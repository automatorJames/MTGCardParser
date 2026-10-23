namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Heart of Kiran, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Heart of Kiran</exampledoc>
[TokenizationOrder(-1)]
public class HeartOfKiranRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may remove a loyalty counter from a planeswalker you control rather than pay", Nib.This, "'s crew cost"];
}
