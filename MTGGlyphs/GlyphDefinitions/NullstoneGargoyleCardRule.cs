namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nullstone Gargoyle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nullstone Gargoyle</exampledoc>
[TokenizationOrder(-1)]
public class NullstoneGargoyleCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever the first noncreature spell of a turn is cast, counter that spell"];
}
