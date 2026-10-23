namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Soul-Shackled Zombie, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soul-Shackled Zombie</exampledoc>
[TokenizationOrder(-1)]
public class SoulShackledZombieRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if at least one creature card was exiled this way, each opponent loses 2 life and you gain 2 life"];
}
