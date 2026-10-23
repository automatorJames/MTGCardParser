namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Invoke Calamity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invoke Calamity</exampledoc>
[TokenizationOrder(-1)]
public class InvokeCalamityCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if those spells would be put into your graveyard, exile them instead"];
}
