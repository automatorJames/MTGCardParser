namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"its activated abilities", "enchanted creature's activated abilities": the activated abilities of a permanent, as what a restriction applies to.</summary>
/// <exampledoc>Arachnus Web</exampledoc>
/// <examplecapture>its activated abilities</examplecapture>
[Dependent]
public class ActivatedAbilitiesOf : Glyph
{
    public override Nib[] Nibs => [Prop(Owner), "activated abilities"];

    public OneOf<Its, EnchantedPossessive, ObjectPossessive> Owner { get; set; }
}
