namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One way of paying for a mana symbol: mana symbols ("{u}"), an amount of any mana ("any two mana"), or life ("2 life").</summary>
/// <exampledoc>Advice from the Fae</exampledoc>
/// <examplecapture>any two mana</examplecapture>
[Dependent]
public class ManaPaymentOption : GlyphOneOf
{
    public ManaCost Mana { get; set; }
    public AnyAmountOfMana AnyMana { get; set; }
    public LifeAmount Life { get; set; }
}
