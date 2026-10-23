namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Galion, Elvenking's Butler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Galion, Elvenking's Butler</exampledoc>
[TokenizationOrder(-1)]
public class GalionElvenkingsButlerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its base power and toughness become equal to galion's power and toughness until end of turn"];
}
