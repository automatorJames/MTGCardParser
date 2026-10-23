namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Exuberant Wolfbear, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Exuberant Wolfbear</exampledoc>
/// <examplecapture>you may have the base power and toughness of target human you control become equal to this creature's power and toughness until end of turn</examplecapture>
[Dependent]
public class ExuberantWolfbearEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have the base power and toughness of target human you control become equal to this creature's power and toughness until end of turn"];
}
