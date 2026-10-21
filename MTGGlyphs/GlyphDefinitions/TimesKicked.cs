namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"time it was kicked": each kicker payment for a multikicker spell or permanent.</summary>
/// <exampledoc>Apex Hawks</exampledoc>
/// <examplecapture>time it was kicked</examplecapture>
[Dependent]
public class TimesKicked : Glyph
{
    public override Nib[] Nibs => ["time", Prop(Spell), "was kicked"];

    public It Spell { get; set; }
}
