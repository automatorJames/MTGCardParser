namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Twist Allegiance, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Twist Allegiance</exampledoc>
[TokenizationOrder(-1)]
public class TwistAllegianceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you and target opponent each gain control of all creatures the other controls until end of turn"];
}
