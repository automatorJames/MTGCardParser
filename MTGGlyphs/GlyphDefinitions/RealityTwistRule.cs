namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Reality Twist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reality Twist</exampledoc>
[TokenizationOrder(-1)]
public class RealityTwistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if tapped for mana, plains produce {r}, swamps produce {g}, mountains produce {w}, and forests produce {b} instead of any other type"];
}
