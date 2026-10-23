namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Icebreaker Kraken, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Icebreaker Kraken</exampledoc>
[TokenizationOrder(-1)]
public class IcebreakerKrakenRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["affinity for snow lands (this spell costs {1} less to cast for each snow land you control"];
}
