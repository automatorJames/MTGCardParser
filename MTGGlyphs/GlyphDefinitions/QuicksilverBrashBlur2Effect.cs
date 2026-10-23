namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Quicksilver, Brash Blur, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quicksilver, Brash Blur</exampledoc>
[Dependent]
public class QuicksilverBrashBlur2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter and a double strike counter on quicksilver"];
}
