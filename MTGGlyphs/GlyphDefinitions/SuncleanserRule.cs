namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Suncleanser, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Suncleanser</exampledoc>
[TokenizationOrder(-1)]
public class SuncleanserRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it can't have counters put on it for as long as this creature remains on the battlefield"];
}
