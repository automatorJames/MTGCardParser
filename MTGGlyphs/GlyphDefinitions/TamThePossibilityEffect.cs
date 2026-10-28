namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tam, the Possibility, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tam, the Possibility</exampledoc>
[Dependent]
public class TamThePossibilityEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["proliferate x times, where x is the number of planeswalker types among planeswalkers you control"];
}
