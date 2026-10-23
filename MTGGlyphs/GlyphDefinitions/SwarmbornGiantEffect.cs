namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Swarmborn Giant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swarmborn Giant</exampledoc>
[Dependent]
public class SwarmbornGiantEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice this creature"];
}
