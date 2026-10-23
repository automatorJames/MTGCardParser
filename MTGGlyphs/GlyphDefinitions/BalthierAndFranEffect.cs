namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Balthier and Fran, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Balthier and Fran</exampledoc>
[Dependent]
public class BalthierAndFranEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's the first combat phase of the turn, you may pay {1}{r}{g}"];
}
