namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mockingbird, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mockingbird</exampledoc>
/// <examplecapture>you may have this creature enter as a copy of any creature on the battlefield with mana value less than or equal to the amount of mana spent to cast this creature, except it's a bird in addition to its other types and it has flying</examplecapture>
[TokenizationOrder(-1)]
public class MockingbirdRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have this creature enter as a copy of any creature on the battlefield with mana value less than or equal to the amount of mana spent to cast this creature, except it's a bird in addition to its other types and it has flying"];
}
