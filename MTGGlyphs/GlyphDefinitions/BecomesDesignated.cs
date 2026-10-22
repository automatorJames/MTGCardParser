namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A creature or card gaining a designation: "this creature becomes prepared", "it becomes prepared", "it becomes plotted".</summary>
/// <exampledoc>Leech Collector // Bloodletting</exampledoc>
/// <examplecapture>becomes prepared</examplecapture>
[Dependent]
public class BecomesDesignated : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("becomes?"), Prop(Designation)];

    public CardDesignation Designation { get; set; }
}
