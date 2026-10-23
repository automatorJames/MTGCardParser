namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A "with the same controller" qualifier, tying a second target to the controller of the first: "another target creature with the same controller", "another permanent with the same controller".</summary>
/// <exampledoc>Simic Guildmage</exampledoc>
/// <examplecapture>with the same controller</examplecapture>
[Dependent]
public class WithSameController : Glyph
{
    public override Nib[] Nibs => ["with the same controller"];
}
