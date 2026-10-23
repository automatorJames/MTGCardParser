namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Taigam, Master Opportunist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Taigam, Master Opportunist</exampledoc>
[Dependent]
public class TaigamMasterOpportunistEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy it, then exile the spell you cast with four time counters on it"];
}
