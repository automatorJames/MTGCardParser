namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Atris, Oracle of Half-Truths, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Atris, Oracle of Half-Truths</exampledoc>
/// <examplecapture>target opponent looks at the top three cards of your library and separates them into a face-down pile and a face-up pile</examplecapture>
[Dependent]
public class AtrisOracleOfHalfTruthsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent looks at the top three cards of your library and separates them into a face-down pile and a face-up pile"];
}
