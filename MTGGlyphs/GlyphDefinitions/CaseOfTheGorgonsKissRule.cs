namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Case of the Gorgon's Kiss, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Case of the Gorgon's Kiss</exampledoc>
[TokenizationOrder(-1)]
public class CaseOfTheGorgonsKissRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["to solve — three or more creature cards were put into graveyards from anywhere this turn"];
}
