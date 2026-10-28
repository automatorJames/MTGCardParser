namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Samut, the Driving Force, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Samut, the Driving Force</exampledoc>
[TokenizationOrder(-1)]
public class SamutTheDrivingForceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["other creatures you control get +x/+0, where x is your speed"];
}
