namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Aura Graft, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aura Graft</exampledoc>
[TokenizationOrder(-1)]
public class AuraGraftRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["attach it to another permanent it can enchant"];
}
