namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Osseous Sticktwister, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Osseous Sticktwister</exampledoc>
[TokenizationOrder(-1)]
public class OsseousSticktwisterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then this creature deals damage equal to its power to each opponent who didn't sacrifice a permanent or discard a card this way"];
}
