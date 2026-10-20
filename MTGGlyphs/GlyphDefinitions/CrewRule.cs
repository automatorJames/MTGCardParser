namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Crew and saddle reminder text: "tap any number of creatures you control with total power 2 or more: this vehicle becomes an artifact creature until end of turn".</summary>
/// <exampledoc>Daredevil Dragster</exampledoc>
/// <examplecapture>tap any number of creatures you control with total power 2 or more: this vehicle becomes an artifact creature until end of turn</examplecapture>
public class CrewRule : Glyph
{
    public override Nib[] Nibs => ["tap any number of", Opt("other"), "creatures you control with total power", Prop(Power), "or more:", Prop(Result), "until end of turn"];

    public Quantity Power { get; set; }
    public CrewResult Result { get; set; }
}
