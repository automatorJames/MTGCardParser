namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Great Train Heist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Great Train Heist</exampledoc>
[Dependent]
public class GreatTrainHeistEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a tapped treasure token"];
}
