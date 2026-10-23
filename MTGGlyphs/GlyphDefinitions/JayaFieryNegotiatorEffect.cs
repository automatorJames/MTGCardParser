namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jaya, Fiery Negotiator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jaya, Fiery Negotiator</exampledoc>
[Dependent]
public class JayaFieryNegotiatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["jaya deals damage equal to the number of attacking creatures to that creature"];
}
