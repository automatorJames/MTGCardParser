namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Melira, the Living Cure, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Melira, the Living Cure</exampledoc>
[Dependent]
public class MeliraTheLivingCureCard3Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose another target creature or artifact"];
}
