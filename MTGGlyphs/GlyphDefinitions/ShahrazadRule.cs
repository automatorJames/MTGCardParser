namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shahrazad, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shahrazad</exampledoc>
[TokenizationOrder(-1)]
public class ShahrazadRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player who doesn't win the subgame loses half their life, rounded up"];
}
