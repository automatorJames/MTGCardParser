namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ancient Cornucopia, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ancient Cornucopia</exampledoc>
[Dependent]
public class AncientCornucopiaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may gain 1 life for each of that spell's colors"];
}
