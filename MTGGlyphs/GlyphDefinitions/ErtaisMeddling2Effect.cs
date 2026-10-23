namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ertai's Meddling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ertai's Meddling</exampledoc>
[Dependent]
public class ErtaisMeddling2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that card is exiled, remove a delay counter from it"];
}
