namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"from anywhere other than your hand": a spell cast from any zone but its caster's hand (exile, graveyard, library, command zone).</summary>
/// <exampledoc>Vega, the Watcher</exampledoc>
/// <examplecapture>from anywhere other than your hand</examplecapture>
[Dependent]
public class CastFromOtherThanHand : Glyph
{
    public override Nib[] Nibs => ["from anywhere other than", Prop(Whose), "hand"];

    public Whose Whose { get; set; }
}
