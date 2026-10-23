namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dyadrine, Synthesis Amalgam, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dyadrine, Synthesis Amalgam</exampledoc>
[TokenizationOrder(-1)]
public class DyadrineSynthesisAmalgamRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["dyadrine enters with a number of +1/+1 counters on it equal to the amount of mana spent to cast it"];
}
