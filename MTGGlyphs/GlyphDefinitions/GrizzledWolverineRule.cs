namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Grizzled Wolverine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grizzled Wolverine</exampledoc>
[TokenizationOrder(-1)]
public class GrizzledWolverineRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["activate only during the declare blockers step, only if at least one creature is blocking this creature, and only once each turn"];
}
