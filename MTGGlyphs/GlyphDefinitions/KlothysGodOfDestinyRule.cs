namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Klothys, God of Destiny, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Klothys, God of Destiny</exampledoc>
[TokenizationOrder(-1)]
public class KlothysGodOfDestinyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, you gain 2 life and klothys deals 2 damage to each opponent"];
}
