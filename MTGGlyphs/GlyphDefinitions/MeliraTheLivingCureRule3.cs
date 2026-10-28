namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Melira, the Living Cure, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Melira, the Living Cure</exampledoc>
[TokenizationOrder(-1)]
public class MeliraTheLivingCureRule3 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when it's put into a graveyard this turn, return that card to the battlefield under its owner's control"];
}
