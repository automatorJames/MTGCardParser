namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vortex Elemental, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vortex Elemental</exampledoc>
[Dependent]
public class VortexElementalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put this creature and each creature blocking or blocked by it on top of their owners' libraries, then those players shuffle"];
}
