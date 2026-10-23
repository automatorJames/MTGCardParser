namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Molten Sentry, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Molten Sentry</exampledoc>
[TokenizationOrder(-1)]
public class MoltenSentryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the coin comes up heads, this creature enters as a 5/2 creature with haste"];
}
