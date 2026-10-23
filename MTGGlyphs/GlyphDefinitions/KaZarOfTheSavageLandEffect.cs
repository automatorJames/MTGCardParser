namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ka-Zar of the Savage Land, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ka-Zar of the Savage Land</exampledoc>
/// <examplecapture>create zabu, a legendary 2/2 green cat creature token with "landfall — whenever a land you control enters, put a +1/+1 counter on zabu</examplecapture>
[Dependent]
public class KaZarOfTheSavageLandEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create zabu, a legendary 2/2 green cat creature token with \"landfall — whenever a land you control enters, put a +1/+1 counter on zabu"];
}
