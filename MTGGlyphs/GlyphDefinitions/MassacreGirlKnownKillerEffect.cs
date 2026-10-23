namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Massacre Girl, Known Killer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Massacre Girl, Known Killer</exampledoc>
[Dependent]
public class MassacreGirlKnownKillerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if its toughness was less than 1, draw a card"];
}
