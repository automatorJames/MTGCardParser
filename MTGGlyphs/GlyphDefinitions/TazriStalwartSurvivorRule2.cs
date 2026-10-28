namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tazri, Stalwart Survivor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tazri, Stalwart Survivor</exampledoc>
[TokenizationOrder(-1)]
public class TazriStalwartSurvivorRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only if this creature has another activated ability"];
}
