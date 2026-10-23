namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Klaw, Sonic Subjugator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Klaw, Sonic Subjugator</exampledoc>
/// <examplecapture>target player reveals a number of cards from their hand equal to one plus the number of creature cards in your graveyard</examplecapture>
[Dependent]
public class KlawSonicSubjugatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player reveals a number of cards from their hand equal to one plus the number of creature cards in your graveyard"];
}
