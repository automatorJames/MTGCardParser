namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Taii Wakeen, Perfect Shot, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Taii Wakeen, Perfect Shot</exampledoc>
[Dependent]
public class TaiiWakeenPerfectShot2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card"];
}
