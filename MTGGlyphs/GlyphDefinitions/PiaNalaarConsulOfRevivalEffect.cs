namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pia Nalaar, Consul of Revival, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pia Nalaar, Consul of Revival</exampledoc>
[Dependent]
public class PiaNalaarConsulOfRevivalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 1/1 colorless thopter artifact creature token with flying"];
}
