namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Reluctant Role Model, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reluctant Role Model</exampledoc>
[Dependent]
public class ReluctantRoleModelEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this creature is tapped, put a flying, lifelink, or +1/+1 counter on it"];
}
