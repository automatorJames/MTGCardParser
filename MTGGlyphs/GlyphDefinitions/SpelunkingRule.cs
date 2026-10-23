namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spelunking, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spelunking</exampledoc>
[TokenizationOrder(-1)]
public class SpelunkingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you put a cave onto the battlefield this way, you gain 4 life"];
}
