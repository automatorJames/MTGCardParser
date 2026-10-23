namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vastwood Hydra, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vastwood Hydra</exampledoc>
/// <examplecapture>you may distribute a number of +1/+1 counters equal to the number of +1/+1 counters on this creature among any number of creatures you control</examplecapture>
[Dependent]
public class VastwoodHydraEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may distribute a number of +1/+1 counters equal to the number of +1/+1 counters on this creature among any number of creatures you control"];
}
