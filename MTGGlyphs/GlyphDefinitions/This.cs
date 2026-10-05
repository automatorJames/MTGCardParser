namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The card's own name, which the corpus writes as "{this}".</summary>
[Dependent]
public class This : Glyph
{
    public override Nib[] Nibs => ["{this}"];
}
