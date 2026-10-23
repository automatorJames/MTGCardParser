namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jungle Patrol, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jungle Patrol</exampledoc>
[Dependent]
public class JunglePatrolEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add {r}"];
}
