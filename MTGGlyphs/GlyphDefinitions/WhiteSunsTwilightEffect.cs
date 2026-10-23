namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of White Sun's Twilight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>White Sun's Twilight</exampledoc>
[Dependent]
public class WhiteSunsTwilightEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["players dealt combat damage by a creature with toxic 1 also get a poison counter"];
}
