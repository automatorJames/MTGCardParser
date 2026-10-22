namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The bare noun "spell", where no qualifiers precede it.</summary>
/// <exampledoc>Highspire Bell-Ringer</exampledoc>
/// <examplecapture>spell</examplecapture>
[Dependent]
public class SpellNoun : Glyph
{
    public override Nib[] Nibs => ["spell"];
}
