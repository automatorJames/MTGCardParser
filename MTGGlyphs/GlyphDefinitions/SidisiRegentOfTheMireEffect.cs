namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sidisi, Regent of the Mire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sidisi, Regent of the Mire</exampledoc>
/// <examplecapture>return target creature card with mana value x plus 1 from your graveyard to the battlefield</examplecapture>
[Dependent]
public class SidisiRegentOfTheMireEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target creature card with mana value x plus 1 from your graveyard to the battlefield"];
}
