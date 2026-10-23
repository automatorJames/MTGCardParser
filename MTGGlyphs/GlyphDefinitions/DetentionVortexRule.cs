namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Detention Vortex, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Detention Vortex</exampledoc>
[TokenizationOrder(-1)]
public class DetentionVortexRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only your opponents may activate this ability and only as a sorcery"];
}
