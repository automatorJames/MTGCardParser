namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Eldrazi Mimic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eldrazi Mimic</exampledoc>
/// <examplecapture>you may have this creature's base power and toughness become equal to that creature's power and toughness until end of turn</examplecapture>
[Dependent]
public class EldraziMimicEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have this creature's base power and toughness become equal to that creature's power and toughness until end of turn"];
}
