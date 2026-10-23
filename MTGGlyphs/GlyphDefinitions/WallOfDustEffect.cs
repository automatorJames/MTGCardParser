namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wall of Dust, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wall of Dust</exampledoc>
[Dependent]
public class WallOfDustEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature can't attack during its controller's next turn"];
}
