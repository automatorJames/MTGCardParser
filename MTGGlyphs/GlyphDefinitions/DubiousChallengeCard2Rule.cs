namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dubious Challenge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dubious Challenge</exampledoc>
[TokenizationOrder(-1)]
public class DubiousChallengeCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put the rest onto the battlefield under your control"];
}
