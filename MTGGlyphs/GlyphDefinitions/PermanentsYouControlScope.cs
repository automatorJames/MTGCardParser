namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"for permanents you control": the permanents whose keyword cost an alternative cost replaces.</summary>
/// <exampledoc>Thick-Skinned Goblin</exampledoc>
/// <examplecapture>for permanents you control</examplecapture>
[Dependent]
public class PermanentsYouControlScope : Glyph
{
    public override Nib[] Nibs => ["for", Prop(Permanents)];

    public Permanents Permanents { get; set; }
}
