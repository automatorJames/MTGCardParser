namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Helm of Obedience, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Helm of Obedience</exampledoc>
/// <examplecapture>target opponent mills a card, then repeats this process until a creature card or x cards have been put into their graveyard this way, whichever comes first</examplecapture>
[Dependent]
public class HelmOfObedienceEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent mills a card, then repeats this process until a creature card or x cards have been put into their graveyard this way, whichever comes first"];
}
