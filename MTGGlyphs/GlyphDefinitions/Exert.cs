namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Exerting a creature as a cost: "exert this creature". An exerted creature won't untap during its controller's next untap step.</summary>
/// <exampledoc>Oasis Ritualist</exampledoc>
/// <examplecapture>exert this creature</examplecapture>
[Dependent]
public class Exert : Glyph
{
    public override Nib[] Nibs => ["exert", Prop(Exerted)];

    public PermanentPhrase Exerted { get; set; }
}
