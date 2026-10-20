namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"no untapped lands", "no other colorless creatures", "no walls": the absence of any permanent of a kind, as a condition's object.</summary>
/// <exampledoc>Keldon Berserker</exampledoc>
/// <examplecapture>no untapped lands</examplecapture>
[Dependent]
public class NoPermanents : Glyph
{
    public override Nib[] Nibs => ["no", Prop(Permanents)];

    public OneOf<OtherPermanents, Permanents> Permanents { get; set; }
}
