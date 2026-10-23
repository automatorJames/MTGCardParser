namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ethersworn Canonist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ethersworn Canonist</exampledoc>
[TokenizationOrder(-1)]
public class EtherswornCanonistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player who has cast a nonartifact spell this turn can't cast additional nonartifact spells"];
}
