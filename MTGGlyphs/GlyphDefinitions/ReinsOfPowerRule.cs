namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Reins of Power, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reins of Power</exampledoc>
[TokenizationOrder(-1)]
public class ReinsOfPowerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you and that opponent each gain control of all creatures the other controls until end of turn"];
}
