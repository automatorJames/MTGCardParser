namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ulasht, the Hate Seed, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ulasht, the Hate Seed</exampledoc>
/// <examplecapture>ulasht enters with a +1/+1 counter on it for each other red creature you control and a +1/+1 counter on it for each other green creature you control</examplecapture>
[TokenizationOrder(-1)]
public class UlashtTheHateSeedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ulasht enters with a +1/+1 counter on it for each other red creature you control and a +1/+1 counter on it for each other green creature you control"];
}
