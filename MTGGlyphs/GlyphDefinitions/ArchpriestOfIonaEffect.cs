namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Archpriest of Iona, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Archpriest of Iona</exampledoc>
/// <examplecapture>if you have a full party, target creature gets +1/+1 and gains flying until end of turn</examplecapture>
[Dependent]
public class ArchpriestOfIonaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you have a full party, target creature gets +1/+1 and gains flying until end of turn"];
}
