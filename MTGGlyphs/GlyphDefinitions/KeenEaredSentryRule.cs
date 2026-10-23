namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Keen-Eared Sentry, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Keen-Eared Sentry</exampledoc>
[TokenizationOrder(-1)]
public class KeenEaredSentryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent can't venture into the dungeon more than once each turn"];
}
