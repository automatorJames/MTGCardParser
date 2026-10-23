namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Coils of the Medusa, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coils of the Medusa</exampledoc>
[Dependent]
public class CoilsOfTheMedusaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all non-wall creatures blocking enchanted creature"];
}
