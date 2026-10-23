namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lunar Convocation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lunar Convocation</exampledoc>
[Dependent]
public class LunarConvocationEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you gained and lost life this turn, create a 1/1 black bat creature token with flying"];
}
