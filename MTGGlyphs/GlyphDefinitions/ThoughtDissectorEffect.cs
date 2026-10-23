namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Thought Dissector, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thought Dissector</exampledoc>
[Dependent]
public class ThoughtDissectorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent reveals cards from the top of their library until an artifact card or x cards are revealed, whichever comes first"];
}
