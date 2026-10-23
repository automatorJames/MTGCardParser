namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Aura Graft, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aura Graft</exampledoc>
[TokenizationOrder(-1)]
public class AuraGraftRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of target aura that's attached to a permanent"];
}
