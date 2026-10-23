namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Might Makes Right, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Might Makes Right</exampledoc>
/// <examplecapture>if you control each creature on the battlefield with the greatest power, gain control of target creature an opponent controls until end of turn</examplecapture>
[Dependent]
public class MightMakesRightEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control each creature on the battlefield with the greatest power, gain control of target creature an opponent controls until end of turn"];
}
