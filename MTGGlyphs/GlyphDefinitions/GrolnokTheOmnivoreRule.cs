namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Grolnok, the Omnivore, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grolnok, the Omnivore</exampledoc>
[TokenizationOrder(-1)]
public class GrolnokTheOmnivoreRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may play lands and cast spells from among cards you own in exile with croak counters on them"];
}
