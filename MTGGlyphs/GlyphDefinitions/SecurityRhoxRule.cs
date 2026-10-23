namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Security Rhox, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Security Rhox</exampledoc>
[TokenizationOrder(-1)]
public class SecurityRhoxRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend only mana produced by treasures to cast it this way"];
}
