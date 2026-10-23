namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jace, Mirror Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jace, Mirror Mage</exampledoc>
/// <examplecapture>if jace was kicked, create a token that's a copy of jace, except it's not legendary and its starting loyalty is 1</examplecapture>
[Dependent]
public class JaceMirrorMageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if jace was kicked, create a token that's a copy of jace, except it's not legendary and its starting loyalty is 1"];
}
