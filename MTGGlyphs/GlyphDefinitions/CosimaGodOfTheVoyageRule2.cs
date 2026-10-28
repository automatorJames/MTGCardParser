namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cosima, God of the Voyage // The Omenkeel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cosima, God of the Voyage // The Omenkeel</exampledoc>
/// <examplecapture>if you don't, return cosima to the battlefield with x +1/+1 counters on it and draw x cards, where x is the number of voyage counters on it</examplecapture>
[TokenizationOrder(-1)]
public class CosimaGodOfTheVoyageRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you don't, return cosima to the battlefield with x +1/+1 counters on it and draw x cards, where x is the number of voyage counters on it"];
}
