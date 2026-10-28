namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Justice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Justice</exampledoc>
[TokenizationOrder(-1)]
public class JusticeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a red creature or spell deals damage, this enchantment deals that much damage to that creature's or spell's controller"];
}
