namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One row of a die-roll result table: "1—9 | effect", "20 | effect". The range is the results it applies to; the effect is any glyph that reads it.</summary>
/// <exampledoc>Goblin Morningstar</exampledoc>
/// <examplecapture>1—9 | create a 1/1 red goblin creature token</examplecapture>
public class DieResultRow : Glyph
{
    public override Nib[] Nibs => [Prop(Results), "|", Prop(Effect)];

    public DieResultRange Results { get; set; }
    public DynamicGlyph Effect { get; set; }
}
