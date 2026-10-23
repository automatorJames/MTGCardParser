namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lyzolda, the Blood Witch, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lyzolda, the Blood Witch</exampledoc>
[TokenizationOrder(-1)]
public class LyzoldaTheBloodWitchCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card if the sacrificed creature was black"];
}
