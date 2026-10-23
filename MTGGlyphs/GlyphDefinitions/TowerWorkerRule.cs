namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tower Worker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tower Worker</exampledoc>
[TokenizationOrder(-1)]
public class TowerWorkerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control creatures named mine worker and power plant worker, add {c}{c}{c} instead"];
}
