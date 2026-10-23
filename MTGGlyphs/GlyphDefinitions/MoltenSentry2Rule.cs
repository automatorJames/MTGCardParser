namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Molten Sentry, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Molten Sentry</exampledoc>
[TokenizationOrder(-1)]
public class MoltenSentry2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it comes up tails, this creature enters as a 2/5 creature with defender"];
}
