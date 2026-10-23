namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Getaway Car, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Getaway Car</exampledoc>
[Dependent]
public class GetawayCarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return up to one target creature that crewed it this turn to its owner's hand"];
}
