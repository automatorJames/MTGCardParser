namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Squad Commander, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Squad Commander</exampledoc>
/// <examplecapture>if you have a full party, creatures you control get +1/+0 and gain indestructible until end of turn</examplecapture>
[Dependent]
public class SquadCommanderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you have a full party, creatures you control get +1/+0 and gain indestructible until end of turn"];
}
