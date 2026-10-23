namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Mycotyrant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Mycotyrant</exampledoc>
[Dependent]
public class TheMycotyrant2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you descend each time a permanent card is put into your graveyard from anywhere"];
}
