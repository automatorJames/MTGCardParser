namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Drill Too Deep, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Drill Too Deep</exampledoc>
[TokenizationOrder(-1)]
public class DrillTooDeepRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• put five charge counters on target spacecraft or planet you control"];
}
