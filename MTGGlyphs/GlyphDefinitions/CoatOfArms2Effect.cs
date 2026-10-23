namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Coat of Arms, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coat of Arms</exampledoc>
[Dependent]
public class CoatOfArms2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for example, if two goblin warriors and a goblin shaman are on the battlefield, each gets +2/+2"];
}
