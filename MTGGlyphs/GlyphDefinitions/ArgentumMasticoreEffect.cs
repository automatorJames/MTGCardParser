namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Argentum Masticore, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Argentum Masticore</exampledoc>
/// <examplecapture>destroy target nonland permanent an opponent controls with mana value less than or equal to the mana value of the discarded card</examplecapture>
[Dependent]
public class ArgentumMasticoreEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target nonland permanent an opponent controls with mana value less than or equal to the mana value of the discarded card"];
}
