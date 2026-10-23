namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thrun, Breaker of Silence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thrun, Breaker of Silence</exampledoc>
[TokenizationOrder(-1)]
public class ThrunBreakerOfSilenceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["thrun can't be the target of nongreen spells your opponents control or abilities from nongreen sources your opponents control"];
}
