namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bartered Cow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bartered Cow</exampledoc>
[Dependent]
public class BarteredCowEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a food token"];
}
