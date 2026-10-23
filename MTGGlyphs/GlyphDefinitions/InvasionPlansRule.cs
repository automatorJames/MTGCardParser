namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Invasion Plans, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invasion Plans</exampledoc>
[TokenizationOrder(-1)]
public class InvasionPlansRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the attacking player chooses how each creature blocks each combat"];
}
