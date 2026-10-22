namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"its activated abilities", "their activated abilities" (detain's plural reminder), "enchanted creature's activated abilities": the activated abilities of a permanent or of permanents named earlier, as what a restriction applies to.</summary>
/// <exampledoc>Arachnus Web</exampledoc>
/// <examplecapture>its activated abilities</examplecapture>
[Dependent]
public class ActivatedAbilitiesOf : Glyph
{
    public override Nib[] Nibs => [Prop(Owner), "activated abilities"];

    public OneOf<Its, EnchantedPossessive, OneOf<ObjectPossessive, Their>> Owner { get; set; }
}
