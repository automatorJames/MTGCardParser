namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Teferi's Time Twist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi's Time Twist</exampledoc>
[TokenizationOrder(-1)]
public class TeferisTimeTwistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it enters as a creature, it enters with an additional +1/+1 counter on it"];
}
