namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sedris, the Traitor King, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sedris, the Traitor King</exampledoc>
[Dependent]
public class SedrisTheTraitorKingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return the card to the battlefield"];
}
