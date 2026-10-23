namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Varolz, the Scar-Striped, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Varolz, the Scar-Striped</exampledoc>
/// <examplecapture>put a number of +1/+1 counters equal to that card's power on target creature</examplecapture>
[Dependent]
public class VarolzTheScarStripedEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a number of +1/+1 counters equal to that card's power on target creature"];
}
