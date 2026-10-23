namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Burning-Rune Demon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Burning-Rune Demon</exampledoc>
[Dependent]
public class BurningRuneDemonEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may search your library for exactly two cards not named", Nib.This, "that have different names"];
}
