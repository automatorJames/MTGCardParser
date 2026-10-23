namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Xathrid Demon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Xathrid Demon</exampledoc>
[TokenizationOrder(-1)]
public class XathridDemonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you can't sacrifice a creature, tap this creature and you lose 7 life"];
}
