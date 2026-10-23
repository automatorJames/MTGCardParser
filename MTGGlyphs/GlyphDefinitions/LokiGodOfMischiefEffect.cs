namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Loki, God of Mischief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Loki, God of Mischief</exampledoc>
[Dependent]
public class LokiGodOfMischiefEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card"];
}
