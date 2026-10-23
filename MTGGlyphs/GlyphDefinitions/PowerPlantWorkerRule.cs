namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Power Plant Worker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Power Plant Worker</exampledoc>
[TokenizationOrder(-1)]
public class PowerPlantWorkerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control creatures named mine worker and tower worker, put two +1/+1 counters on this creature instead"];
}
