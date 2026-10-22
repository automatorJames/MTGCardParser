namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that source", "the source": a damage source named earlier in the line ("whenever a source deals damage to this creature, that source's controller …").</summary>
/// <exampledoc>Phyrexian Obliterator</exampledoc>
/// <examplecapture>that source</examplecapture>
[Dependent]
public class ThatSource : BackReference<DamageSourceNoun>
{
    public override Nib[] Nibs => [Alt("that", "the"), "source"];
}
