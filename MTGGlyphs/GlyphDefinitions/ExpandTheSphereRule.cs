namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Expand the Sphere, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Expand the Sphere</exampledoc>
[TokenizationOrder(-1)]
public class ExpandTheSphereRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you put fewer than two lands onto the battlefield this way, proliferate a number of times equal to the difference"];
}
