namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The legendary sorcery rule, as its reminder text puts it: "you may cast a legendary sorcery only if you control a legendary creature or planeswalker".</summary>
/// <exampledoc>Primevals' Glorious Rebirth</exampledoc>
/// <examplecapture>you may cast a legendary sorcery only if you control a legendary creature or planeswalker</examplecapture>
public class LegendarySorceryRule : Glyph
{
    public override Nib[] Nibs => ["you may cast a legendary sorcery only if you control a legendary creature or planeswalker"];
}
