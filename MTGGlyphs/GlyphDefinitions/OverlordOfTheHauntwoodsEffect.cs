namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Overlord of the Hauntwoods, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Overlord of the Hauntwoods</exampledoc>
[Dependent]
public class OverlordOfTheHauntwoodsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a tapped colorless land token named everywhere that is every basic land type"];
}
