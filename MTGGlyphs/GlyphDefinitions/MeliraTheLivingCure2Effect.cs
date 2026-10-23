namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Melira, the Living Cure, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Melira, the Living Cure</exampledoc>
[Dependent]
public class MeliraTheLivingCure2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return that card to the battlefield under its owner's control"];
}
