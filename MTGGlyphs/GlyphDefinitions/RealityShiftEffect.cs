namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Reality Shift, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reality Shift</exampledoc>
[Dependent]
public class RealityShiftEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player puts the top card of their library onto the battlefield face down as a 2/2 creature"];
}
