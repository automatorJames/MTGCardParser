namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Minion of the Wastes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Minion of the Wastes</exampledoc>
/// <examplecapture>{this}'s power and toughness are each equal to the life paid as it entered</examplecapture>
[TokenizationOrder(-1)]
public class MinionOfTheWastesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "'s power and toughness are each equal to the life paid as it entered"];
}
