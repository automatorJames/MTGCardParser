namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Agency Outfitter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Agency Outfitter</exampledoc>
/// <examplecapture>you may search your graveyard, hand and/or library for a card named magnifying glass and/or a card named thinking cap and put them onto the battlefield</examplecapture>
[Dependent]
public class AgencyOutfitterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may search your graveyard, hand and/or library for a card named magnifying glass and/or a card named thinking cap and put them onto the battlefield"];
}
