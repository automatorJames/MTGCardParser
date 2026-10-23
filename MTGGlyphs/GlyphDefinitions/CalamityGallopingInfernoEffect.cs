namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Calamity, Galloping Inferno, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Calamity, Galloping Inferno</exampledoc>
/// <examplecapture>choose a nonlegendary creature that saddled it this turn and create a tapped and attacking token that's a copy of it</examplecapture>
[Dependent]
public class CalamityGallopingInfernoEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a nonlegendary creature that saddled it this turn and create a tapped and attacking token that's a copy of it"];
}
