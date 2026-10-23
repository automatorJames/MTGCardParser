namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Grip of Chaos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grip of Chaos</exampledoc>
[Dependent]
public class GripOfChaosEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it has a single target, reselect its target at random"];
}
