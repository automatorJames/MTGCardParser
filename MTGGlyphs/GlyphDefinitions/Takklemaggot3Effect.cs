namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Takklemaggot, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Takklemaggot</exampledoc>
[Dependent]
public class Takklemaggot3Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature's controller chooses a creature that this card could enchant"];
}
