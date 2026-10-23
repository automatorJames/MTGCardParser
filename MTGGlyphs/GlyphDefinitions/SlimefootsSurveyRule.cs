namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Slimefoot's Survey, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Slimefoot's Survey</exampledoc>
/// <examplecapture>domain — search your library for up to two land cards that each have a basic land type, put them onto the battlefield tapped, then shuffle</examplecapture>
[TokenizationOrder(-1)]
public class SlimefootsSurveyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["domain — search your library for up to two land cards that each have a basic land type, put them onto the battlefield tapped, then shuffle"];
}
