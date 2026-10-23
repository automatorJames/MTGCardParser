namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Codespell Cleric, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Codespell Cleric</exampledoc>
[Dependent]
public class CodespellClericEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it was the second spell you cast this turn, put a +1/+1 counter on target creature"];
}
