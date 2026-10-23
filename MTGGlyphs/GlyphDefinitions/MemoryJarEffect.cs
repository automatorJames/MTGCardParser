namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Memory Jar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Memory Jar</exampledoc>
/// <examplecapture>each player discards their hand and returns to their hand each card they exiled this way</examplecapture>
[Dependent]
public class MemoryJarEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player discards their hand and returns to their hand each card they exiled this way"];
}
