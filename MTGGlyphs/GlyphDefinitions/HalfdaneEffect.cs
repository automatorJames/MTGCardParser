namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Halfdane, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Halfdane</exampledoc>
/// <examplecapture>{this}'s base power and toughness become equal to the power and toughness of target creature other than {this} until the end of your next upkeep</examplecapture>
[Dependent]
public class HalfdaneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => [Nib.This, "'s base power and toughness become equal to the power and toughness of target creature other than", Nib.This, "until the end of your next upkeep"];
}
