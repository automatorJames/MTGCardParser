namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Gluttonous Troll, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gluttonous Troll</exampledoc>
[Dependent]
public class GluttonousTrollEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["food tokens are artifacts with \"{2}, {t}, sacrifice this token: you gain 3 life"];
}
