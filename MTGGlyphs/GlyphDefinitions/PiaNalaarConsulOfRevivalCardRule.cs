namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pia Nalaar, Consul of Revival, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pia Nalaar, Consul of Revival</exampledoc>
[TokenizationOrder(-1)]
public class PiaNalaarConsulOfRevivalCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you play a land from exile or cast a spell from exile, create a 1/1 colorless thopter artifact creature token with flying"];
}
