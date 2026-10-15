namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent being, or not being, something: "{this} is tapped", "enchanted land is untapped", "it isn't a creature".</summary>
/// <exampledoc>Bottomless Vault</exampledoc>
/// <examplecapture>{this} is tapped</examplecapture>
[Dependent]
public class PermanentIs : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Prop(Assertion), Opt(Pattern("an?")), Prop(Aspect)];

    public PermanentPhrase Subject { get; set; }
    public Assertion Assertion { get; set; }
    public CardAspect Aspect { get; set; }
}
