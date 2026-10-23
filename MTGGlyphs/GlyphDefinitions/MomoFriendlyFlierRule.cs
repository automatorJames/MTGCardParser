namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Momo, Friendly Flier, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Momo, Friendly Flier</exampledoc>
[TokenizationOrder(-1)]
public class MomoFriendlyFlierRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the first non-lemur creature spell with flying you cast during each of your turns costs {1} less to cast"];
}
