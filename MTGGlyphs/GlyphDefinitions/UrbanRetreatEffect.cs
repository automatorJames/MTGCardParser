namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Urban Retreat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Urban Retreat</exampledoc>
[Dependent]
public class UrbanRetreatEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put this card from your hand onto the battlefield"];
}
