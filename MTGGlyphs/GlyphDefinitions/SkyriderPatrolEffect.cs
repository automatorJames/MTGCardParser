namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Skyrider Patrol, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Skyrider Patrol</exampledoc>
[Dependent]
public class SkyriderPatrolEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on another target creature you control, and that creature gains flying until end of turn"];
}
