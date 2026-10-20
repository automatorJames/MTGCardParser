namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A reflexive trigger, on the player doing the optional thing just offered: "when you do, destroy target nonland permanent".</summary>
/// <exampledoc>Ruthless Lawbringer</exampledoc>
/// <examplecapture>when you do, destroy target nonland permanent</examplecapture>
public class ReflexiveTrigger : Glyph
{
    public override Nib[] Nibs => ["when", Prop(Player), "do,", Prop(Effect)];

    public WhichPlayer Player { get; set; }
    public DynamicGlyph Effect { get; set; }
}
