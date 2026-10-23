namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Curator of Destinies, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Curator of Destinies</exampledoc>
[Dependent]
public class CuratorOfDestiniesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["look at the top five cards of your library and separate them into a face-down pile and a face-up pile"];
}
