namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Melira, the Living Cure, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Melira, the Living Cure</exampledoc>
[TokenizationOrder(-1)]
public class MeliraTheLivingCureCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile melira: choose another target creature or artifact"];
}
