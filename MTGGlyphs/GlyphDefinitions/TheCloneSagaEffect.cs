namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Clone Saga, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Clone Saga</exampledoc>
[Dependent]
public class TheCloneSagaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy it, except the copy isn't legendary"];
}
