namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Silverquill, the Disputant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Silverquill, the Disputant</exampledoc>
[Dependent]
public class SilverquillTheDisputantEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as you cast that spell, you may sacrifice a creature with power 1 or greater"];
}
