namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Plague of Vermin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Plague of Vermin</exampledoc>
[TokenizationOrder(-1)]
public class PlagueOfVerminRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["starting with you, each player may pay any amount of life"];
}
