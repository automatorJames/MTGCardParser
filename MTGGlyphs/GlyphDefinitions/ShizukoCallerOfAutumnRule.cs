namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shizuko, Caller of Autumn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shizuko, Caller of Autumn</exampledoc>
[TokenizationOrder(-1)]
public class ShizukoCallerOfAutumnRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, they don't lose this mana as steps and phases end"];
}
