namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ob Nixilis, Captive Kingpin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ob Nixilis, Captive Kingpin</exampledoc>
[Dependent]
public class ObNixilisCaptiveKingpinEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on ob nixilis"];
}
