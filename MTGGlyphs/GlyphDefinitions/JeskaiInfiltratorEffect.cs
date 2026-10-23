namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jeskai Infiltrator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jeskai Infiltrator</exampledoc>
/// <examplecapture>exile it and the top card of your library in a face-down pile, shuffle that pile, then manifest those cards</examplecapture>
[Dependent]
public class JeskaiInfiltratorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile it and the top card of your library in a face-down pile, shuffle that pile, then manifest those cards"];
}
