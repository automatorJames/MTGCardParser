namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Throne of the Grim Captain // The Grim Captain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Throne of the Grim Captain // The Grim Captain</exampledoc>
/// <examplecapture>craft with a dinosaur, a merfolk, a pirate, and a vampire {4} ({4}, exile this artifact, exile the four from among permanents you control and/or cards in your graveyard: return this card transformed under its owner's control</examplecapture>
[TokenizationOrder(-1)]
public class ThroneOfTheGrimCaptainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["craft with a dinosaur, a merfolk, a pirate, and a vampire {4} ({4}, exile this artifact, exile the four from among permanents you control and/or cards in your graveyard: return this card transformed under its owner's control"];
}
