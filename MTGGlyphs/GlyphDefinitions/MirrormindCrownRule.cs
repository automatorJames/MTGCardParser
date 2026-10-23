namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mirrormind Crown, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirrormind Crown</exampledoc>
/// <examplecapture>as long as this equipment is attached to a creature, the first time you would create one or more tokens each turn, you may instead create that many tokens that are copies of equipped creature</examplecapture>
[TokenizationOrder(-1)]
public class MirrormindCrownRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as this equipment is attached to a creature, the first time you would create one or more tokens each turn, you may instead create that many tokens that are copies of equipped creature"];
}
