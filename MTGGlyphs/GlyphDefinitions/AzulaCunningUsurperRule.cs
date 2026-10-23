namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Azula, Cunning Usurper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Azula, Cunning Usurper</exampledoc>
[TokenizationOrder(-1)]
public class AzulaCunningUsurperRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during your turn, you may cast cards exiled with azula and you may cast them as though they had flash"];
}
