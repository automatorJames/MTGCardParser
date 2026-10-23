namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Neurok Transmuter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Neurok Transmuter</exampledoc>
[Dependent]
public class NeurokTransmuterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, target artifact creature becomes blue and isn't an artifact"];
}
