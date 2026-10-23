namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Colossus of the Blood Age, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Colossus of the Blood Age</exampledoc>
[Dependent]
public class ColossusOfTheBloodAgeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["discard any number of cards, then draw that many cards plus one"];
}
