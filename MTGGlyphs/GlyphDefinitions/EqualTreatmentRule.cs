namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Equal Treatment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Equal Treatment</exampledoc>
/// <examplecapture>if any source would deal 1 or more damage to a permanent or player this turn, it deals 2 damage to that permanent or player instead</examplecapture>
[TokenizationOrder(-1)]
public class EqualTreatmentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if any source would deal 1 or more damage to a permanent or player this turn, it deals 2 damage to that permanent or player instead"];
}
