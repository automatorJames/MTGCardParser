namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rat King, Verminister, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rat King, Verminister</exampledoc>
/// <examplecapture>return target creature card and all other cards with the same name as that card from your graveyard to the battlefield tapped</examplecapture>
[Dependent]
public class RatKingVerministerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target creature card and all other cards with the same name as that card from your graveyard to the battlefield tapped"];
}
