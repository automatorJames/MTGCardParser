namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Parallel Thoughts, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Parallel Thoughts</exampledoc>
[Dependent]
public class ParallelThoughts2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for seven cards, exile them in a face-down pile, and shuffle that pile"];
}
