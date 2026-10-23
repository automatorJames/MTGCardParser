namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Intrepid Outlander, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Intrepid Outlander</exampledoc>
[Dependent]
public class IntrepidOutlanderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you attacked with creatures with total power 6 or greater this combat, venture into the dungeon"];
}
