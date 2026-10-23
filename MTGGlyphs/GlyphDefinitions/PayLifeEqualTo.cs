namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"pay life equal to this creature's power": paying life equal to an object's characteristic as a keyword's whole cost (ward). Offered only where a keyword's action cost goes, since anything reachable from Cost is expanded everywhere.</summary>
/// <exampledoc>Phyrexian Fleshgorger</exampledoc>
/// <examplecapture>pay life equal to this creature's power</examplecapture>
[Dependent]
public class PayLifeEqualTo : Glyph
{
    public override Nib[] Nibs => ["pay life equal to", Prop(Measure)];

    public CharacteristicOf Measure { get; set; }
}
