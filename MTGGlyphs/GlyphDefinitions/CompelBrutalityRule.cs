namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Compel Brutality, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Compel Brutality</exampledoc>
[TokenizationOrder(-1)]
public class CompelBrutalityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• target planeswalker you control deals damage equal to its loyalty to target creature or planeswalker an opponent controls"];
}
