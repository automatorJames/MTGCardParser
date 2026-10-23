namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Archfiend's Vessel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Archfiend's Vessel</exampledoc>
[Dependent]
public class ArchfiendsVesselEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it entered from your graveyard or you cast it from your graveyard, exile it"];
}
