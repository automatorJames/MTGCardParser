namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mine Worker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mine Worker</exampledoc>
[TokenizationOrder(-1)]
public class MineWorkerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control creatures named power plant worker and tower worker, you gain 3 life instead"];
}
