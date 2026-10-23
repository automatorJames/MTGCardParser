namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Where some mana came from, after an amount of it: "from creatures", "from a treasure".</summary>
/// <exampledoc>Inga and Esika</exampledoc>
/// <examplecapture>from creatures</examplecapture>
[Dependent]
public class ManaSourceQualifier : Glyph
{
    public override Nib[] Nibs => ["from", Prop(Source)];

    public OneOf<IndefinitePermanent, Permanents> Source { get; set; }
}
