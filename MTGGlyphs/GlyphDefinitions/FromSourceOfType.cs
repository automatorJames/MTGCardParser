namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Where an ability on the stack comes from, by its source's type: "from an artifact source", "from a creature source".</summary>
/// <exampledoc>Weaver of Harmony</exampledoc>
/// <examplecapture>from an enchantment source</examplecapture>
[Dependent]
public class FromSourceOfType : Glyph
{
    public override Nib[] Nibs => ["from", Pattern("an?"), Prop(Kind), "source"];

    public CardType Kind { get; set; }
}
