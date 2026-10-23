namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Quicksilver, Brash Blur, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quicksilver, Brash Blur</exampledoc>
[TokenizationOrder(-1)]
public class QuicksilverBrashBlurRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if", Nib.This, "is in your opening hand, you may begin the game with him on the battlefield"];
}
