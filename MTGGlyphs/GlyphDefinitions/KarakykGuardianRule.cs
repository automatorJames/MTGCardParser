namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Karakyk Guardian, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Karakyk Guardian</exampledoc>
[TokenizationOrder(-1)]
public class KarakykGuardianRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has hexproof as long as it hasn't dealt damage yet"];
}
