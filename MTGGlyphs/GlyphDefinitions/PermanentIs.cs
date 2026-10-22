namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent being, or not being, something, or one of several things: "{this} is tapped", "enchanted land is untapped", "it isn't a creature", "that creature is black or red", "enchanted creature is white and untapped", "purphoros isn't a creature" (a legend's short name, tried last).</summary>
/// <exampledoc>Bottomless Vault</exampledoc>
/// <examplecapture>{this} is tapped</examplecapture>
[Dependent]
public class PermanentIs : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Prop(Assertion), Opt(Pattern("an?")), Prop(Aspect)];

    public OneOf<PermanentPhrase, ShortName> Subject { get; set; }
    public Assertion Assertion { get; set; }
    public OneOf<ManyOf<CardAspect>, CardAspect> Aspect { get; set; }
}
