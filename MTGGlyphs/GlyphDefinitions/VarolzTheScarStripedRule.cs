namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Varolz, the Scar-Striped, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Varolz, the Scar-Striped</exampledoc>
[TokenizationOrder(-1)]
public class VarolzTheScarStripedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["(exile a creature card from your graveyard and pay its mana cost: put a number of +1/+1 counters equal to that card's power on target creature"];
}
