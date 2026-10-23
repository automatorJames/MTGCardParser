namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cherished Hatchling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cherished Hatchling</exampledoc>
/// <examplecapture>you may cast dinosaur spells this turn as though they had flash, and whenever you cast a dinosaur spell this turn, it gains "when this creature enters, you may have it fight another target creature</examplecapture>
[Dependent]
public class CherishedHatchlingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast dinosaur spells this turn as though they had flash, and whenever you cast a dinosaur spell this turn, it gains \"when this creature enters, you may have it fight another target creature"];
}
