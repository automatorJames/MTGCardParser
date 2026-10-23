namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Swallowed by Leviathan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swallowed by Leviathan</exampledoc>
[TokenizationOrder(-1)]
public class SwallowedByLeviathanRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["surveil 2, then counter the chosen spell unless its controller pays {1} for each card in your graveyard"];
}
