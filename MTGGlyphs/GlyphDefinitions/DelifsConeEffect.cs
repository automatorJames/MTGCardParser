namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Delif's Cone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Delif's Cone</exampledoc>
[Dependent]
public class DelifsConeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this turn, when target creature you control attacks and isn't blocked, you may gain life equal to its power"];
}
