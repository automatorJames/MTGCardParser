namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sorcerer Class, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sorcerer Class</exampledoc>
/// <examplecapture>that spell deals damage to each opponent equal to the number of instant and sorcery spells you've cast this turn</examplecapture>
[Dependent]
public class SorcererClassEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that spell deals damage to each opponent equal to the number of instant and sorcery spells you've cast this turn"];
}
