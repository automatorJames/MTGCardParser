namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Agency Coroner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Agency Coroner</exampledoc>
[TokenizationOrder(-1)]
public class AgencyCoronerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the sacrificed creature was suspected, draw two cards instead"];
}
