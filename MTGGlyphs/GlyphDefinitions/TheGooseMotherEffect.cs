namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of The Goose Mother, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Goose Mother</exampledoc>
[Dependent]
public class TheGooseMotherEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create half x food tokens, rounded up"];
}
