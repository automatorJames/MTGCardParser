namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Reckoner Bankbuster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reckoner Bankbuster</exampledoc>
/// <examplecapture>then if there are no charge counters on this vehicle, create a treasure token and a 1/1 colorless pilot creature token with "this token crews vehicles as though its power were 2 greater</examplecapture>
[TokenizationOrder(-1)]
public class ReckonerBankbusterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then if there are no charge counters on this vehicle, create a treasure token and a 1/1 colorless pilot creature token with \"this token crews vehicles as though its power were 2 greater"];
}
