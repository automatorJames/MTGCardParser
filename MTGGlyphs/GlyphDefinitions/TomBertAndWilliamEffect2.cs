namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tom, Bert, and William, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tom, Bert, and William</exampledoc>
[Dependent]
public class TomBertAndWilliamEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they were a creature, return them to the battlefield"];
}
