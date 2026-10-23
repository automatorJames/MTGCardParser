namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Creeping Dread, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Creeping Dread</exampledoc>
[TokenizationOrder(-1)]
public class CreepingDreadRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent who discarded a card that shares a card type with the card you discarded loses 3 life"];
}
