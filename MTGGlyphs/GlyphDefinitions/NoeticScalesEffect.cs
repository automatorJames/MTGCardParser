namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Noetic Scales, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Noetic Scales</exampledoc>
/// <examplecapture>return to its owner's hand each creature that player controls with power greater than the number of cards in their hand</examplecapture>
[Dependent]
public class NoeticScalesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return to its owner's hand each creature that player controls with power greater than the number of cards in their hand"];
}
