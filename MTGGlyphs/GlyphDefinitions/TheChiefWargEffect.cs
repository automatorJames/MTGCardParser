namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Chief Warg, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Chief Warg</exampledoc>
[Dependent]
public class TheChiefWargEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you draw a card and lose 1 life"];
}
