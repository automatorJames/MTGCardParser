namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Security Bypass, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Security Bypass</exampledoc>
[TokenizationOrder(-1)]
public class SecurityBypassRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they discarded a nonland card, they put a +1/+1 counter on this creature"];
}
