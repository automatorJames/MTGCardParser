namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Visage of Dread // Dread Osseosaur, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Visage of Dread // Dread Osseosaur</exampledoc>
/// <examplecapture>craft with two creatures {5}{b} ({5}{b}, exile this artifact, exile the two from among creatures you control and/or creature cards in your graveyard: return this card transformed under its owner's control</examplecapture>
[TokenizationOrder(-1)]
public class VisageOfDreadRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["craft with two creatures {5}{b} ({5}{b}, exile this artifact, exile the two from among creatures you control and/or creature cards in your graveyard: return this card transformed under its owner's control"];
}
