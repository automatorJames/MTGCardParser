namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Justice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Justice</exampledoc>
[Dependent]
public class JusticeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this enchantment deals that much damage to that creature's or spell's controller"];
}
