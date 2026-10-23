namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Silverquill, the Disputant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Silverquill, the Disputant</exampledoc>
[Dependent]
public class SilverquillTheDisputant2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy the spell and you may choose new targets for the copy"];
}
