namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Planeswalker's Mischief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Planeswalker's Mischief</exampledoc>
[Dependent]
public class PlaneswalkersMischiefEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you haven't cast it, return it to its owner's hand"];
}
