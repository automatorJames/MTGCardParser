namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Increment's reminder text.</summary>
/// <exampledoc>Cuboid Colony</exampledoc>
/// <examplecapture>whenever you cast a spell, if the amount of mana you spent is greater than this creature's power or toughness, put a +1/+1 counter on this creature</examplecapture>
public class IncrementRule : Glyph
{
    public override Nib[] Nibs => ["whenever you cast a spell, if the amount of mana you spent is greater than this creature's power or toughness, put a +1/+1 counter on this creature"];
}
