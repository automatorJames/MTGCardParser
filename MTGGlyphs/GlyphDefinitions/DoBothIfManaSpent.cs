namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The hybrid-spend reminder: "do both if {w}{b} was spent" - both conditional effects happen if mana of both colors was spent.</summary>
/// <exampledoc>Firespout</exampledoc>
/// <examplecapture>do both if {r}{g} was spent</examplecapture>
public class DoBothIfManaSpent : Glyph
{
    public override Nib[] Nibs => ["do both if", Prop(Mana), "was spent"];

    public ManaCost Mana { get; set; }
}
