namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Polukranos, Unchained, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Polukranos, Unchained</exampledoc>
/// <examplecapture>if damage would be dealt to polukranos while it has a +1/+1 counter on it, prevent that damage and remove that many +1/+1 counters from it</examplecapture>
[TokenizationOrder(-1)]
public class PolukranosUnchainedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if damage would be dealt to polukranos while it has a +1/+1 counter on it, prevent that damage and remove that many +1/+1 counters from it"];
}
