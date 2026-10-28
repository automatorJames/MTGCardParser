namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"your hand", "their hand", "target player's hand": a whole hand, as what's looked at or revealed.</summary>
/// <exampledoc>Peek</exampledoc>
/// <examplecapture>target player's hand</examplecapture>
[Dependent]
public class PlayersHand : Glyph
{
    public override Nib[] Nibs => [Prop(Owner), "hand"];

    public Owner Owner { get; set; }
}
