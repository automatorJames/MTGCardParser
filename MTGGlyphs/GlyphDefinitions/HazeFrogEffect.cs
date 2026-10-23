namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Haze Frog, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Haze Frog</exampledoc>
[Dependent]
public class HazeFrogEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all combat damage that other creatures would deal this turn"];
}
