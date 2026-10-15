namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Monstrosity's reminder text.</summary>
/// <exampledoc>Nemesis of Mortals</exampledoc>
/// <examplecapture>if this creature isn't monstrous, put three +1/+1 counters on it and it becomes monstrous</examplecapture>
public class MonstrosityRule : Glyph
{
    public override Nib[] Nibs => ["if this creature isn't monstrous, put", Prop(Counters), "on it and it becomes monstrous"];

    public Counters Counters { get; set; }
}
