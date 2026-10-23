namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Superior Spider-Man, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Superior Spider-Man</exampledoc>
/// <examplecapture>mind swap — you may have {this} enter as a copy of any creature card in a graveyard, except his name is {this} and he's a 4/4 spider human hero in addition to his other types</examplecapture>
[TokenizationOrder(-1)]
public class SuperiorSpiderManRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["mind swap — you may have", Nib.This, "enter as a copy of any creature card in a graveyard, except his name is", Nib.This, "and he's a 4/4 spider human hero in addition to his other types"];
}
