namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mangara's Tome, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mangara's Tome</exampledoc>
[Dependent]
public class MangarasTome2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for five cards, exile them in a face-down pile, and shuffle that pile"];
}
