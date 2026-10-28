namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wall of Caltrops, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wall of Caltrops</exampledoc>
/// <examplecapture>if at least one other wall creature is blocking that creature and no non-wall creatures are blocking that creature, this creature gains banding until end of turn</examplecapture>
[Dependent]
public class WallOfCaltropsEffect2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if at least one other wall creature is blocking that creature and no non-wall creatures are blocking that creature, this creature gains banding until end of turn"];
}
