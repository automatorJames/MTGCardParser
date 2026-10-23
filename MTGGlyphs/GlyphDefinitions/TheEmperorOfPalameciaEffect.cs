namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Emperor of Palamecia // The Lord Master of Hell, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Emperor of Palamecia // The Lord Master of Hell</exampledoc>
[Dependent]
public class TheEmperorOfPalameciaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if at least four mana was spent to cast it, put a +1/+1 counter on the emperor of palamecia"];
}
