namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Minimus Containment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Minimus Containment</exampledoc>
[Dependent]
public class MinimusContainmentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it was a creature, it's no longer a creature"];
}
