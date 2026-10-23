namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Duplicant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Duplicant</exampledoc>
/// <examplecapture>as long as a card exiled with this creature is a creature card, this creature has the power, toughness, and creature types of the last creature card exiled with it</examplecapture>
[TokenizationOrder(-1)]
public class DuplicantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as a card exiled with this creature is a creature card, this creature has the power, toughness, and creature types of the last creature card exiled with it"];
}
