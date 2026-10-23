namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Eccentric Apprentice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eccentric Apprentice</exampledoc>
/// <examplecapture>if you've completed a dungeon, up to one target creature becomes a bird with base power and toughness 1/1 and flying until end of turn</examplecapture>
[Dependent]
public class EccentricApprenticeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you've completed a dungeon, up to one target creature becomes a bird with base power and toughness 1/1 and flying until end of turn"];
}
