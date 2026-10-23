namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Triton Tactics, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Triton Tactics</exampledoc>
/// <examplecapture>at this turn's next end of combat, tap each creature that was blocked by one of those creatures this turn and it doesn't untap during its controller's next untap step</examplecapture>
[TokenizationOrder(-1)]
public class TritonTacticsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["at this turn's next end of combat, tap each creature that was blocked by one of those creatures this turn and it doesn't untap during its controller's next untap step"];
}
