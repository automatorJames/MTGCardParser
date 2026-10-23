namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cosima, God of the Voyage // The Omenkeel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cosima, God of the Voyage // The Omenkeel</exampledoc>
[TokenizationOrder(-1)]
public class CosimaGodOfTheVoyage2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, it gains \"whenever a land you control enters, if cosima is exiled, you may put a voyage counter on it"];
}
