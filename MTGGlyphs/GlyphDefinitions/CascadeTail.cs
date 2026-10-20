namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cascade's reminder text, middle sentence: casting the exiled card for free. The last sentence is parsed by PutIntoZone. Tried first, so the general PlayCard predicate doesn't take it.</summary>
/// <exampledoc>Ardent Plea</exampledoc>
/// <examplecapture>you may cast it without paying its mana cost</examplecapture>
[TokenizationOrder(0)]
public class CascadeTail : Glyph
{
    public override Nib[] Nibs => ["you may cast it without paying its mana cost"];
}
