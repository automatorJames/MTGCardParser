namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"created with this enchantment", "created with this creature": tokens this permanent made earlier, as a qualifier on a group of permanents (Tombstone Stairwell, Saproling Burst, Tetravus).</summary>
/// <exampledoc>Dual Nature</exampledoc>
/// <examplecapture>created with this enchantment</examplecapture>
[Dependent]
public class CreatedWithThis : Glyph
{
    public override Nib[] Nibs => ["created with", Prop(Source)];

    public OneOf<ThisCreature, This> Source { get; set; }
}
