namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A reconfigured equipment's status while attached: "while attached, this isn't a creature".</summary>
/// <exampledoc>Simian Sling</exampledoc>
/// <examplecapture>while attached, this isn't a creature</examplecapture>
public class WhileAttachedNotCreature : Glyph
{
    public override Nib[] Nibs => ["while attached,", Prop(Object), "isn't a", Prop(Kind)];

    public ThisCreature Object { get; set; }
    public CardType Kind { get; set; }
}
