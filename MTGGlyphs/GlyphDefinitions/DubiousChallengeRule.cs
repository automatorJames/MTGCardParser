namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dubious Challenge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dubious Challenge</exampledoc>
[TokenizationOrder(-1)]
public class DubiousChallengeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent may choose one of the exiled cards and put it onto the battlefield under their control"];
}
