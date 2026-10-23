namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Alrund, God of the Cosmos // Hakka, Whispering Raven, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alrund, God of the Cosmos // Hakka, Whispering Raven</exampledoc>
[TokenizationOrder(-1)]
public class AlrundGodOfTheCosmosRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["alrund gets +1/+1 for each card in your hand and each foretold card you own in exile"];
}
