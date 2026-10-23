namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Chandra's Regulator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra's Regulator</exampledoc>
[Dependent]
public class ChandrasRegulatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card"];
}
