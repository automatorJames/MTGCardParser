namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Grip of Chaos, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grip of Chaos</exampledoc>
[Dependent]
public class GripOfChaosCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["select from among all legal targets"];
}
