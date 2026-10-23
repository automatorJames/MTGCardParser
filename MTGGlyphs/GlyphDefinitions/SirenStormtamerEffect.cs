namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Siren Stormtamer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Siren Stormtamer</exampledoc>
[Dependent]
public class SirenStormtamerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell or ability that targets you or a creature you control"];
}
