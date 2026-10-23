namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sarkhan, the Dragonspeaker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sarkhan, the Dragonspeaker</exampledoc>
[Dependent]
public class SarkhanTheDragonspeakerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["he doesn't lose loyalty while he's not a planeswalker"];
}
