namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Grip of Chaos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grip of Chaos</exampledoc>
[TokenizationOrder(-1)]
public class GripOfChaosRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a spell or ability is put onto the stack, if it has a single target, reselect its target at random"];
}
