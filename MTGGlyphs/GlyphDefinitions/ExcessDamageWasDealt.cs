namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Whether the damage effect just described dealt excess damage, as a condition: "excess damage was dealt this way", "excess damage was dealt to that creature this way".</summary>
/// <exampledoc>Orbital Plunge</exampledoc>
/// <examplecapture>excess damage was dealt this way</examplecapture>
[Dependent]
public class ExcessDamageWasDealt : Glyph
{
    public override Nib[] Nibs => ["excess damage", Pattern("was|is"), Prop(Dealt)];

    public ExcessDealt Dealt { get; set; }
}
