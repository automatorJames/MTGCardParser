namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"target player's hand".</summary>
/// <exampledoc>Clairvoyance</exampledoc>
/// <examplecapture>target player's hand</examplecapture>
[Dependent]
public class LookedAtHand : Glyph
{
    public override Nib[] Nibs => [Prop(Owner), "hand"];

    public Owner Owner { get; set; }
}
