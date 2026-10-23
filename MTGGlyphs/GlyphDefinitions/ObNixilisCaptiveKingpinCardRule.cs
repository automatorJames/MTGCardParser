namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ob Nixilis, Captive Kingpin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ob Nixilis, Captive Kingpin</exampledoc>
[TokenizationOrder(-1)]
public class ObNixilisCaptiveKingpinCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever one or more opponents each lose exactly 1 life, put a +1/+1 counter on ob nixilis"];
}
