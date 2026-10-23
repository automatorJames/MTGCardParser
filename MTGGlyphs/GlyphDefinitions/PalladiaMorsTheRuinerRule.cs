namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Palladia-Mors, the Ruiner, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Palladia-Mors, the Ruiner</exampledoc>
[TokenizationOrder(-1)]
public class PalladiaMorsTheRuinerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["palladia-mors has hexproof as long as it hasn't dealt damage yet"];
}
