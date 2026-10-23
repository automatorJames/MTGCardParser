namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Captive Audience, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Captive Audience</exampledoc>
[TokenizationOrder(-1)]
public class CaptiveAudienceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment enters under the control of an opponent of your choice"];
}
