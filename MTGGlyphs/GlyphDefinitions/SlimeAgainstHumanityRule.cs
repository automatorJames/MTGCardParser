namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Slime Against Humanity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Slime Against Humanity</exampledoc>
/// <examplecapture>put x +1/+1 counters on it, where x is two plus the total number of cards you own in exile and in your graveyard that are oozes or are named {this}</examplecapture>
[TokenizationOrder(-1)]
public class SlimeAgainstHumanityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put x +1/+1 counters on it, where x is two plus the total number of cards you own in exile and in your graveyard that are oozes or are named", Nib.This];
}
