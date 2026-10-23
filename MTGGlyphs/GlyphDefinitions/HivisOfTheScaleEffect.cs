namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hivis of the Scale, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hivis of the Scale</exampledoc>
[Dependent]
public class HivisOfTheScaleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of target dragon for as long as you control hivis and hivis remains tapped"];
}
