namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Witchbane Orb, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Witchbane Orb</exampledoc>
[Dependent]
public class WitchbaneOrbCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all curses attached to you"];
}
