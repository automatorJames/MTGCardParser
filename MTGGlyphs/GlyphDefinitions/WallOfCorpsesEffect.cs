namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wall of Corpses, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wall of Corpses</exampledoc>
[Dependent]
public class WallOfCorpsesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target creature this creature is blocking"];
}
