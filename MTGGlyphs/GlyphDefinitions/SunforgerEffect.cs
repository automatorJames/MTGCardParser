namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sunforger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sunforger</exampledoc>
/// <examplecapture>search your library for a red or white instant card with mana value 4 or less and cast that card without paying its mana cost</examplecapture>
[Dependent]
public class SunforgerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for a red or white instant card with mana value 4 or less and cast that card without paying its mana cost"];
}
