namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tam, the Possibility, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tam, the Possibility</exampledoc>
[Dependent]
public class TamThePossibilityEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["to proliferate, choose any number of permanents and/or players, then give each another counter of each kind already there"];
}
