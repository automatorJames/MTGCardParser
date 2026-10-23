namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Eternity Vessel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eternity Vessel</exampledoc>
[Dependent]
public class EternityVesselEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have your life total become the number of charge counters on this artifact"];
}
