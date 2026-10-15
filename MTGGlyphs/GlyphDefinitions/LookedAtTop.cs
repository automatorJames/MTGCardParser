namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the top four cards of target player's library".</summary>
/// <exampledoc>Natural Selection</exampledoc>
/// <examplecapture>the top three cards of target player's library</examplecapture>
[Dependent]
public class LookedAtTop : Glyph
{
    public override Nib[] Nibs => ["the top", Prop(Quantity), Plural("card"), "of", Prop(Owner), "library"];

    public Quantity Quantity { get; set; }
    public Owner Owner { get; set; }
}
